using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using SalesWeb.Extensions;

namespace SalesWeb.Services
{
    /// <summary>
    /// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
    /// with UI-specific types (view models and SelectListItem types).
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogIllustration> _illustrationRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IUriComposer _uriComposer;
        private readonly GroceryContext _db; //TODO move all queries to repo

        public CatalogService(
            ILoggerFactory loggerFactory,
            IRepository<CatalogItem> itemRepository,
            IAsyncRepository<CatalogIllustration> illustrationRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IUriComposer uriComposer,
            GroceryContext db)
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
            var root = _itemRepository
                .List(filterSpecification);

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
                    ProductSku = i.Sku
                }),
                //Illustrations = await GetIllustrations(),
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

        //public async Task<IEnumerable<SelectListItem>> GetIllustrations()
        //{
        //    _logger.LogInformation("GetIllustrations called.");
        //    var illustrations = await _illustrationRepository.ListAllAsync();

        //    var items = new List<SelectListItem>
        //    {
        //        new SelectListItem() { Value = null, Text = "Todos", Selected = true }
        //    };
        //    foreach (CatalogIllustration item in illustrations)
        //    {
        //        items.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name });
        //    }

        //    return items;
        //}

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

        public async Task<CatalogIndexViewModel> GetCategoryCatalogItems(int categoryId, int pageIndex, int? itemsPage)
        {
            //TODO: Move to repo

            //Get Category Name
            var category = await _db.Categories.FindAsync(categoryId);

            var filterSpecification = new CatalogFilterSpecification(null, null, categoryId);
            var root = _itemRepository
                .List(filterSpecification);

            var totalItems = root.Count();

            var iPage = itemsPage ?? totalItems;

            var allItems = root.ToList();

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
                    CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price ?? i.CatalogType.Price,
                        ProductSku = i.Sku,
                        CatalogTypeName = i.CatalogType?.Description
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
                return vm;
            }
            return null;
        }

        public async Task<ProductViewModel> GetCatalogItem(string sku)
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
                .SingleOrDefaultAsync(x => x.Sku == sku);

            if (product != null)
            {
                List<ProductReferenceViewModel> productReferences = new List<ProductReferenceViewModel>();
                if (product.CatalogReferences?.Count > 0)
                {
                    productReferences.Add(new ProductReferenceViewModel { Label = product.CatalogReferences.First().LabelDescription, Name = product.Name, Sku = product.Sku });
                    productReferences.AddRange(product.CatalogReferences
                        .Select(x => new ProductReferenceViewModel
                        {
                            Label = x.LabelDescription,
                            Name = x.ReferenceCatalogItem.Name,
                            Sku = x.ReferenceCatalogItem.Sku
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
                        TagName = Utils.StringToUri(x.Category.Name)
                    }).ToList(),
                    Tags = new List<LinkViewModel>
                    {
                        new LinkViewModel { Name = product.CatalogType.Description, TagName = "tipo"},
                        new LinkViewModel { Name = product.CatalogIllustration.Name, TagName = "ilustracao"},
                        new LinkViewModel { Name = product.CatalogIllustration.IllustrationType.Name, TagName = "ilustracao_tipo"},
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
                    }).ToList()
                };

                //Others prictures
                if (product.CatalogPictures.Where(x => x.IsActive).Count() > 0)
                    vm.ProductImagesUri.AddRange(
                        product.CatalogPictures
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.Order)
                        .Select(x => x.PictureUri)
                        );

                //Attributes
                //decimal attrPriceDefault = 0M;
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
                        //Attributes = grpAttr.Select(x => new AttributeViewModel
                        //{
                        //    Id = x.Id,
                        //    Price = x.Price,
                        //    Sku = x.Sku,
                        //}).ToList()
                    });
                }
                //Categories
                //foreach (var item in product.CatalogType.Categories)
                //{
                //    vm.Categories.Add(new LinkViewModel
                //    {
                //        Name = item.Category.Name,
                //        TagName = Utils.StringToUri(item.Category.Name)
                //    });
                //}
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

        //public async Task<AttributeViewModel> GetAttributeDetails(int attributeId)
        //{
        //    var attr = await _db.CatalogAttributes
        //        .Include(x => x.ReferenceCatalogItem)
        //        .SingleOrDefaultAsync(x => x.Id == attributeId);
        //    if (attr != null)
        //        return new AttributeViewModel
        //        {
        //            Price = attr.Price,
        //            ReferenceCatalogSku = attr.ReferenceCatalogItem?.Sku
        //        };
        //    return null;
        //}

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
                            .Where(x => Utils.StringToUri(x.CatalogType.Description) == tagName);
                        break;
                    case TagType.ILLUSTRATION:
                        query = _db.CatalogItems
                            .Include(x => x.CatalogType)
                            .Include(x => x.CatalogIllustration)
                            .Where(x => Utils.StringToUri(x.CatalogIllustration.Name) == tagName);
                        break;
                    case TagType.ILLUSTRATION_TYPE:
                        query = _db.CatalogItems
                            .Include(x => x.CatalogType)
                            .Include(x => x.CatalogIllustration)
                            .ThenInclude(ci => ci.IllustrationType)
                            .Where(x => x.CatalogIllustration.IllustrationType.Name == tagName);
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
                    .Where(x => Utils.StringToUri(x.CatalogType.Description) == tagName || Utils.StringToUri(x.CatalogIllustration.Name) == tagName || x.CatalogIllustration.IllustrationType.Name == tagName);
            }

            query = query.Where(x => x.ShowOnShop && (!illustrationId.HasValue || x.CatalogIllustrationId == illustrationId) &&
                (!typeId.HasValue || x.CatalogTypeId == typeId));

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
                    ProductSku = x.Sku
                }).ToList(),
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = iPage != 0 ? int.Parse(Math.Ceiling(((decimal)totalItems / iPage)).ToString()) : 0
                },
                //Illustrations = await GetIllustrations(),
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
                //.Include(x => x.CatalogIllustration)
                //.ThenInclude(ci => ci.IllustrationType)
                .Where(x => x.ShowOnShop &&
                (!typeId.HasValue || x.CatalogTypeId == typeId) &&
                (x.CatalogType.Description.Contains(searchFor) ||
                x.Name.Contains(searchFor)));
                //x.Description.Contains(searchFor)));            
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
                    ProductSku = x.Sku
                }).ToList(),
                PaginationInfo = new PaginationInfoViewModel()
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = iPage != 0 ? int.Parse(Math.Ceiling(((decimal)totalItems / iPage)).ToString()) : 0
                },
                //Illustrations = await GetIllustrations(),
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
                //.Include(x => x.CatalogTypes)
                //    .ThenInclude(cts => cts.CatalogType)
                .Where(x => x.CatalogCategories.Count > 0)
                .OrderBy(x => x.Order)
                .ToListAsync();

            List<MenuItemComponentViewModel> menuViewModel = new List<MenuItemComponentViewModel>();

            var parents = categories
                .Where(x => !x.ParentId.HasValue)
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(menuViewModel, categories, parents);

            return menuViewModel;
        }

        private void GetTopCategories(List<MenuItemComponentViewModel> model, List<Category> categories, List<Category> parents)
        {
            model.AddRange(parents.Select(x => new MenuItemComponentViewModel
            {
                Id = x.Id,
                Name = x.Name.ToUpper(),
                NameUri = Utils.RemoveDiacritics(x.Name).Replace(" ", "-").ToLower()
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
                        NameUri = Utils.RemoveDiacritics(x.Name).Replace(" ", "-").ToLower()
                    }));
                } 
                else
                {
                    //var types = categories
                    //    .Where(x => x.Id == item.Id)
                    //    .Select(x => x.CatalogTypes);

                    //Get Catalog Types of Category
                    var category = categories.SingleOrDefault(x => x.Id == item.Id);
                    var types = new List<CatalogType>();
                    //foreach (var catalogType in category.CatalogTypes)
                    //{
                    //    //Check if has items
                    //    var filterSpecification = new CatalogFilterSpecification(null, catalogType.CatalogTypeId, category.Id);
                    //    var items = _itemRepository.List(filterSpecification).ToList();

                    //    if (items?.Count() > 0)
                    //    {
                    //        if (!types.Any(x => x.Id == catalogType.CatalogTypeId))
                    //            types.Add(catalogType.CatalogType);
                    //    }
                    //}
                    //var catalogTypes = types.SelectMany(x => x.Select(t => t.CatalogType));

                    if (types?.Count() >= 0)
                    {
                        item.Childs.AddRange(types.OrderBy(x => x.Description).Select(x => new MenuItemComponentViewModel
                        {
                            Id = x.Id,
                            Name = x.Description.ToUpper(),
                            NameUri = Utils.RemoveDiacritics(item.Name).Replace(" ", "-").ToLower(),
                            TypeUri = Utils.RemoveDiacritics(x.Description).Replace(" ", "-").ToLower()
                        }));
                    }
                }
            }
        }

        public async Task<(int, string)?> GetCatalogType(string type)
        {
            var allCatalogTypes = await _db.CatalogTypes.ToListAsync();
            foreach (var item in allCatalogTypes)
            {
                var typeName = item.Description.Replace(" ", "-").ToLower();
                if (Utils.RemoveDiacritics(typeName) == type)
                    return (item.Id, item.Description);
            }
            return null;
        }

        public async Task<(int, string)?> GetCategory(string name)
        {
            var allCategories = await _db.Categories.ToListAsync();
            foreach (var item in allCategories)
            {
                var catName = item.Name.Replace(" ", "-").ToLower();
                var normalize = Utils.RemoveDiacritics(catName);
                if (normalize == name.ToLower())
                    return (item.Id, item.Name);
            }
            return null;
        }
    }
}
