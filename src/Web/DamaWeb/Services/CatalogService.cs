using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using DamaWeb.Extensions;
using ApplicationCore;

namespace DamaWeb.Services
{
    /// <summary>
    /// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
    /// with UI-specific types (view models and SelectListItem types).
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogIllustration> _illustrationRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IUriComposer _uriComposer;
        private readonly DamaContext _db; //TODO move all queries to repo

        public CatalogService(
            ILoggerFactory loggerFactory,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<CatalogIllustration> illustrationRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IUriComposer uriComposer,
            DamaContext db)
        {
            _logger = loggerFactory.CreateLogger<CatalogService>();
            _itemRepository = itemRepository;
            _illustrationRepository = illustrationRepository;
            _typeRepository = typeRepository;
            _uriComposer = uriComposer;
            _db = db;
        }

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? illustrationId, int? typeId, int? categoryId)
        {
            _logger.LogInformation("GetCatalogItems called.");

            var filterSpecification = new CatalogFilterSpecification(illustrationId, typeId, categoryId);
            var root = await _itemRepository
                .ListAsync(filterSpecification);

            var totalItems = root.Count();

            var iPage = itemsPage ?? totalItems;

            var itemsOnPage = root
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToList();

            itemsOnPage.ForEach(x =>
            {
                x.PictureUri = _uriComposer.ComposePicUri(x.PictureUri);
            });

            var vm = new CatalogIndexViewModel()
            {
                CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
                {
                    CatalogItemId = i.Id,
                    CatalogItemName = i.Name,
                    PictureUri = i.PictureUri,
                    Price = i.Price ?? i.CatalogType.Price,
                    ProductSlug = i.Slug
                    //ProductSku = i.Sku
                }),
                NewCatalogItems = itemsOnPage
                    .Where(x => x.IsNew)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price ?? i.CatalogType.Price,
                        ProductSlug = i.Slug
                        //ProductSku = i.Sku,
                    }),
                FeaturedCatalogItems = itemsOnPage
                    .Where(x => x.IsFeatured)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price ?? i.CatalogType.Price,
                        ProductSlug = i.Slug
                        //ProductSku = i.Sku,
                    }),
                Illustrations = await GetIllustrations(),
                Types = await GetTypes(),
                IllustrationFilterApplied = illustrationId ?? 0,
                TypesFilterApplied = typeId ?? 0,
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = iPage != 0 ? int.Parse(Math.Ceiling(((decimal)totalItems / iPage)).ToString()) : 0
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return vm;
        }

        public async Task<IEnumerable<SelectListItem>> GetIllustrations()
        {
            _logger.LogInformation("GetIllustrations called.");
            var illustrations = await _illustrationRepository.ListAllAsync();

            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Todos", Selected = true }
            };
            foreach (CatalogIllustration item in illustrations)
            {
                items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            _logger.LogInformation("GetTypes called.");
            var types = await _typeRepository.ListAllAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Todos", Selected = true }
            };
            foreach (CatalogType type in types)
            {
                items.Add(new SelectListItem() { Value = type.Id.ToString(), Text = type.Description });
            }

            return items;
        }

        public async Task<CategoryViewModel> GetCategoryCatalogItems(string categorySlug, int pageIndex, int? itemsPage)
        {
            Category category = await GetCategoryFromUrl(categorySlug);
            if (category == null)
                return null;

            var filterSpecification = new CatalogFilterSpecification(null, null, category.Id);
            var root = await _itemRepository
                .ListAsync(filterSpecification);

            var totalItems = root.Count();

            var iPage = itemsPage ?? totalItems;

            var allItems = root.ToList();

            //Get Catalog Types of Category
            var types = new List<CatalogType>();
            foreach (var item in allItems)
            {
                if (item.CatalogType.CatalogItems
                    .Where(x => x.CatalogCategories.Any(c => c.CategoryId == category.Id))
                    .ToList().Count() > 0)
                {
                    if (!types.Any(x => x.Id == item.CatalogTypeId))
                        types.Add(item.CatalogType);
                }
            }

            var itemsOnPage = allItems
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToList();

            itemsOnPage.ForEach(x =>
            {
                x.PictureUri = _uriComposer.ComposePicUri(x.PictureUri);
            });

            if (allItems?.Count > 0)
            {
                var vm = new CatalogIndexViewModel()
                {
                    NewCatalogItems = allItems
                    .Where(x => x.IsNew)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price ?? i.CatalogType.Price,
                        ProductSlug = i.Slug
                        //ProductSku = i.Sku,
                    }),
                    FeaturedCatalogItems = allItems
                    .Where(x => x.IsFeatured)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price ?? i.CatalogType.Price,
                        ProductSlug = i.Slug
                        //ProductSku = i.Sku
                    }),
                    CatalogTypes = types.OrderBy(x => x.Description).Select(x => new CatalogTypeViewModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Description,
                        PictureUri = x.PictureUri,
                        CatNameUri = category.Slug,
                        TypeNameUri = x.Slug
                    })
                    .ToList(),
                    CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price ?? i.CatalogType.Price,
                        ProductSlug = i.Slug
                        //ProductSku = i.Sku
                    }),
                    PaginationInfo = new PaginationInfoViewModel()
                    {
                        ActualPage = pageIndex,
                        ItemsPerPage = itemsOnPage.Count,
                        TotalItems = totalItems,
                        TotalPages = iPage != 0 ? int.Parse(Math.Ceiling(((decimal)totalItems / iPage)).ToString()) : 0
                    }
                };
                vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
                vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";
                return new CategoryViewModel
                {
                    CatalogModel = vm,
                    CategoryName = category.Name,
                    CategoryUrlName = categorySlug,
                    MetaDescription = category.MetaDescription,
                    Title = string.IsNullOrEmpty(category.Title) ? category.Name : category.Title
                };
            }
            return null;
        }


        public async Task<ProductViewModel> GetCatalogItem(string slug)
        {
            var product = await _db.CatalogItems
                .Include(x => x.CatalogCategories)
                    .ThenInclude(cc => cc.Category)
                .Include(x => x.CatalogPictures)
                .Include(x => x.CatalogAttributes)
                .Include(x => x.CatalogType)
                .Include(x => x.CatalogIllustration)
                    .ThenInclude(ct => ct.IllustrationType)
                .Include(x => x.CatalogType)
                    .ThenInclude(ct => ct.PictureTextHelpers)
                .Include(x => x.CatalogReferences)
                    .ThenInclude(cr => cr.ReferenceCatalogItem)
                .SingleOrDefaultAsync(x => x.Slug == slug);

            if (product != null)
            {
                List<ProductReferenceViewModel> productReferences = new List<ProductReferenceViewModel>();
                if (product.CatalogReferences?.Count > 0)
                {
                    productReferences.Add(new ProductReferenceViewModel { Label = product.CatalogReferences.First().LabelDescription, Name = product.Name, Slug = product.Slug });
                    productReferences.AddRange(product.CatalogReferences
                        .Select(x => new ProductReferenceViewModel
                        {
                            Label = x.LabelDescription,
                            Name = x.ReferenceCatalogItem.Name,
                            Sku = x.ReferenceCatalogItem.Sku,
                            Slug = x.ReferenceCatalogItem.Slug
                        })
                        .ToList());
                }
                var vm = new ProductViewModel
                {
                    ProductId = product.Id,
                    CategoryId = product.CatalogCategories.FirstOrDefault().CategoryId,
                    ProductTypeId = product.CatalogTypeId,
                    ProductSKU = product.Sku,
                    ProductTitle = product.Name,
                    ProductDescription = product.Description,
                    ProductPrice = product.Price ?? product.CatalogType.Price,
                    ProductQuantity = 1,
                    ProductImagesUri = new List<string>
                    {
                        product.PictureUri
                    },
                    Attributes = new List<ProductAttributeViewModel>(),
                    Categories = product.CatalogCategories.Select(x => new LinkViewModel
                    {
                        Name = x.Category.Name,
                        TagName = x.Category.Slug
                    }).ToList(),
                    Tags = new List<LinkViewModel>
                    {
                        new LinkViewModel { Name = product.CatalogType.Description, Uri = product.CatalogType.Slug, TagName = "tipo"},
                        new LinkViewModel { Name = product.CatalogIllustration.Name, Uri = Utils.URLFriendly(product.CatalogIllustration.Name), TagName = "ilustracao"},
                        new LinkViewModel { Name = product.CatalogIllustration.IllustrationType.Name, Uri = Utils.URLFriendly(product.CatalogIllustration.IllustrationType.Name), TagName = "ilustracao_tipo"},
                    },
                    DeliveryTimeMin = product.CatalogType.DeliveryTimeMin,
                    DeliveryTimeMax = product.CatalogType.DeliveryTimeMax,
                    DeliveryTimeUnit = product.CatalogType.DeliveryTimeUnit,
                    CanCustomizeTotal = product.CanCustomize,
                    CustomizePrice = product.CatalogType.AdditionalTextPrice,
                    FirstCategoryId = product.CatalogCategories.FirstOrDefault()?.CategoryId ?? 0,
                    ProductReferences = productReferences,
                    PictureHelpers = product.CatalogType.PictureTextHelpers.Select(x => new PictureHelperViewModel
                    {
                        PictureUri = x.PictureUri,
                        PictureFileName = x.FileName
                    }).ToList(),
                    MetaDescription = product.MetaDescription,
                    Title = string.IsNullOrEmpty(product.Title) ? product.Name : product.Title
                };

                //Others prictures
                if (product.CatalogPictures.Where(x => x.IsActive && !x.IsMain).Count() > 0)
                    vm.ProductImagesUri.AddRange(
                        product.CatalogPictures
                        .Where(x => x.IsActive && !x.IsMain)
                        .OrderBy(x => x.Order)
                        .Select(x => x.PictureUri)
                        );

                //Attributes
                foreach (var grpAttr in product.CatalogAttributes.GroupBy(x => x.Type))
                {
                    //attrPriceDefault += grpAttr.First().Price ?? 0;
                    vm.Attributes.Add(new ProductAttributeViewModel
                    {
                        AttributeType = grpAttr.Key,
                        Items = new SelectList(grpAttr.ToList(), "Id", "Name"),
                        Label = EnumHelper<AttributeType>.GetDisplayValue(grpAttr.Key),
                        DefaultText = GetDefaultText(grpAttr.Key),
                        Selected = grpAttr.First().Id
                    });
                }
                return vm;
            }
            return null;
        }

        private string GetDefaultText(AttributeType key)
        {
            switch (key)
            {
                case AttributeType.SIZE:
                    return "Escolha um tamanho";
                case AttributeType.BOOK_FORMAT:
                    return "Escolha um formato";
                case AttributeType.Color:
                    return "Escolha uma cor";
                case AttributeType.OPTION:
                    return "Escolha uma opção";
                case AttributeType.PET:
                    return "Escolha um animal de estimação";
                case AttributeType.TEXT:
                    return "Escolha uma frase";
                default:
                    return null;
            }
        }

        public async Task<CatalogIndexViewModel> GetCatalogItemsByTag(int pageIndex, int? itemsPage, string tagName, TagType? tagType, int? typeId, int? illustrationId)
        {
            tagName = tagName.ToLower().Trim();
            IQueryable<CatalogItem> query = null;
            if (tagType.HasValue)
            {
                switch (tagType.Value)
                {
                    case TagType.CATALOG_TYPE:
                        query = _db.CatalogItems
                            .Include(x => x.CatalogType)
                            .Where(x => Utils.URLFriendly(x.CatalogType.Description) == tagName);
                        break;
                    case TagType.ILLUSTRATION:
                        query = _db.CatalogItems
                            .Include(x => x.CatalogType)
                            .Include(x => x.CatalogIllustration)
                            .Where(x => Utils.URLFriendly(x.CatalogIllustration.Name) == tagName);
                        break;
                    case TagType.ILLUSTRATION_TYPE:
                        query = _db.CatalogItems
                            .Include(x => x.CatalogType)
                            .Include(x => x.CatalogIllustration)
                            .ThenInclude(ci => ci.IllustrationType)
                            .Where(x => Utils.URLFriendly(x.CatalogIllustration.IllustrationType.Name) == tagName);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                query = _db.CatalogItems
                    .Include(x => x.CatalogType)
                    .Include(x => x.CatalogIllustration)
                    .ThenInclude(ci => ci.IllustrationType)
                    .Where(x => Utils.URLFriendly(x.CatalogType.Description) == tagName || Utils.URLFriendly(x.CatalogIllustration.Name) == tagName || Utils.URLFriendly(x.CatalogIllustration.IllustrationType.Name) == tagName);
            }

            query = query.Where(x => x.ShowOnShop && (!illustrationId.HasValue || x.CatalogIllustrationId == illustrationId) &&
                (!typeId.HasValue || x.CatalogTypeId == typeId));

            var totalItems = query.Count();
            if (totalItems == 0)
                return null;
            var iPage = itemsPage ?? totalItems;
            var itemsOnPage = await query
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToListAsync();

            var vm = new CatalogIndexViewModel
            {
                CatalogItems = itemsOnPage.Select(x => new CatalogItemViewModel
                {
                    CatalogItemId = x.Id,
                    CatalogItemName = x.Name,
                    PictureUri = x.PictureUri,
                    Price = x.Price ?? x.CatalogType.Price,
                    ProductSlug = x.Slug
                    //ProductSku = x.Sku
                }).ToList(),
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = iPage != 0 ? int.Parse(Math.Ceiling(((decimal)totalItems / iPage)).ToString()) : 0
                },
                Illustrations = await GetIllustrations(),
                Types = await GetTypes(),
            };
            vm.PaginationInfo.Next = (vm.PaginationInfo.TotalItems == 0 || vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";
            return vm;
        }
        public async Task<CatalogIndexViewModel> GetCatalogItemsBySearch(int pageIndex, int? itemsPage, string searchFor, int? typeId, int? illustrationId)
        {
            searchFor = searchFor.ToLower().Trim();

            IQueryable<CatalogItem> query = null;
            query = _db.CatalogItems
                .Include(x => x.CatalogType)
                .Include(x => x.CatalogIllustration)
                .ThenInclude(ci => ci.IllustrationType)
                .Where(x => x.ShowOnShop && (!illustrationId.HasValue || x.CatalogIllustrationId == illustrationId) &&
                (!typeId.HasValue || x.CatalogTypeId == typeId) &&
                (x.CatalogType.Description.Contains(searchFor) ||
                x.CatalogIllustration.Name.Contains(searchFor) ||
                x.CatalogIllustration.IllustrationType.Name.Contains(searchFor) ||
                x.Name.Contains(searchFor) ||
                x.Description.Contains(searchFor)));

            var totalItems = query.Count();
            var iPage = itemsPage ?? totalItems;
            var itemsOnPage = await query
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToListAsync();


            var vm = new CatalogIndexViewModel
            {
                CatalogItems = itemsOnPage.Select(x => new CatalogItemViewModel
                {
                    CatalogItemId = x.Id,
                    CatalogItemName = x.Name,
                    PictureUri = x.PictureUri,
                    Price = x.Price ?? x.CatalogType.Price,
                    ProductSlug = x.Slug
                    //ProductSku = x.Sku
                }).ToList(),
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = iPage != 0 ? int.Parse(Math.Ceiling(((decimal)totalItems / iPage)).ToString()) : 0
                },
                Illustrations = await GetIllustrations(),
                Types = await GetTypes(),
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";
            return vm;
        }

        public async Task<List<MenuItemComponentViewModel>> GetMenuViewModel()
        {
            var categories = await _db.Categories
                .Include(x => x.Parent)
                .Include(x => x.CatalogCategories)
                    .ThenInclude(cc => cc.CatalogItem)
                .Include(x => x.CatalogTypes)
                    .ThenInclude(cts => cts.CatalogType)
                        //.ThenInclude(ct => ct.CatalogItems)
                .Where(x => x.CatalogCategories.Count > 0)
                .OrderBy(x => x.Order)
                .ToListAsync();

            List<MenuItemComponentViewModel> menuViewModel = new List<MenuItemComponentViewModel>();

            var parents = categories
                .Where(x => !x.ParentId.HasValue)
                .OrderBy(x => x.Order)
                .ToList();

            await GetTopCategoriesAsync(menuViewModel, categories, parents);

            return menuViewModel;
        }

        private async Task GetTopCategoriesAsync(List<MenuItemComponentViewModel> model, List<Category> categories, List<Category> parents)
        {
            model.AddRange(parents.Select(x => new MenuItemComponentViewModel
            {
                Id = x.Id,
                Name = x.Name.ToUpper(),
                NameUri = x.Slug
            }));

            //SubCategories
            foreach (var item in model)
            {
                var childs = categories
                    .Where(x => x.ParentId == item.Id)
                    .OrderBy(x => x.Order)
                    .ToList();

                if (childs?.Count > 0)
                {
                    item.Childs.AddRange(childs.Select(x => new MenuItemComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name.ToUpper(),
                        NameUri = x.Slug
                    }));
                } 
                else
                {
           
                    //Get Catalog Types of Category
                    var category = categories.SingleOrDefault(x => x.Id == item.Id);
                    var types = new List<CatalogType>();
                    foreach (var catalogType in category.CatalogTypes)
                    {
                        //Check if has items
                        var filterSpecification = new CatalogFilterSpecification(null, catalogType.CatalogTypeId, category.Id);
                        var items = await _itemRepository.ListAsync(filterSpecification);

                        if (items?.Count() > 0)
                        {
                            if (!types.Any(x => x.Id == catalogType.CatalogTypeId))
                                types.Add(catalogType.CatalogType);
                        }
                    }
                    //var catalogTypes = types.SelectMany(x => x.Select(t => t.CatalogType));

                    if (types?.Count() >= 0)
                    {
                        item.Childs.AddRange(types.OrderBy(x => x.Description).Select(x => new MenuItemComponentViewModel
                        {
                            Id = x.Id,
                            Name = x.Description.ToUpper(),
                            NameUri = item.NameUri,
                            TypeUri = x.Slug
                        }));
                    }
                }
            }
        }

        public async Task<CatalogTypeViewModel> GetCatalogTypeItemsAsync(string cat, string type, int pageIndex, int itemsPage)
        {
            var category = await GetCategoryFromUrl(cat);

            CatalogType catalogType = await GetCatalogTypeFromUrl(type);

            if (catalogType == null || category == null)
                return null;

            return new CatalogTypeViewModel
            {
                Name = catalogType.Description,
                CatalogModel = await GetCatalogItems(pageIndex, itemsPage, null, catalogType.Id, category.Id),
                MetaDescription = catalogType.MetaDescription,
                Title = string.IsNullOrEmpty(catalogType.Title) ? catalogType.Description : catalogType.Title
            };
        }
        private async Task<Category> GetCategoryFromUrl(string categorySlug)
        {
            var allCategories = await _db.Categories.ToListAsync();
            return allCategories.SingleOrDefault(x => x.Slug == categorySlug.ToLower());
        }

        private async Task<CatalogType> GetCatalogTypeFromUrl(string type)
        {
            var allCatalogTypes= await _db.CatalogTypes.ToListAsync();
            return allCatalogTypes.SingleOrDefault(x => x.Slug == type);
        }

        public async Task<string> GetSlugFromSkuAsync(string sku)
        {
            var spec = new CatalogSkuSpecification(sku);
            var catalogItem = await _itemRepository.GetSingleBySpecAsync(spec);
            if (catalogItem != null)
                return catalogItem.Slug;
            return string.Empty;
        }
    }
}
