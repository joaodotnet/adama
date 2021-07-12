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
using System.Net;
using Microsoft.Extensions.Hosting;

namespace DamaWeb.Services
{
    /// <summary>
    /// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
    /// with UI-specific types (view models and SelectListItem types).
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IRepository<CatalogIllustration> _illustrationRepository;
        private readonly IRepository<CatalogType> _typeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IHostEnvironment _environment;

        public CatalogService(
            ILoggerFactory loggerFactory,
            IRepository<CatalogItem> itemRepository,
            IRepository<CatalogIllustration> illustrationRepository,
            IRepository<CatalogType> typeRepository,
            IRepository<Category> categoryRepository,
            IHostEnvironment environment)
        {
            _logger = loggerFactory.CreateLogger<CatalogService>();
            _itemRepository = itemRepository;
            _illustrationRepository = illustrationRepository;
            _typeRepository = typeRepository;
            _categoryRepository = categoryRepository;
            _environment = environment;
        }

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? illustrationId, int? typeId, int? categoryId, string onlyAvailable = null)
        {
            _logger.LogInformation("GetCatalogItems called.");

            var filterSpecification = new CatalogFilterSpecification(illustrationId, typeId, categoryId, showOnlyAvailable: onlyAvailable == "true");
            var root = await _itemRepository
                .ListAsync(filterSpecification);

            var totalItems = root.Count();

            var iPage = itemsPage ?? totalItems;

            var itemsOnPage = root
                .OrderByDescending(x => x.Id)
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToList();

            //itemsOnPage.ForEach(x =>
            //{
            //    x.PictureUri = _uriComposer.ComposePicUri(x.PictureUri);
            //});

            var vm = new CatalogIndexViewModel()
            {
                CatalogItems = itemsOnPage
                .Select(i => new CatalogItemViewModel()
                {
                    CatalogItemId = i.Id,
                    CatalogItemName = i.Name,
                    PictureUri = i.PictureUri,
                    PictureHighUri = i.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri,
                    Price = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) - i.Discount.Value : (i.Price ?? i.CatalogType.Price),
                    PriceBeforeDiscount = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) : default(decimal?),
                    ProductSlug = i.Slug,
                    IsUnavailable = i.IsUnavailable
                }),
                NewCatalogItems = new List<CatalogItemViewModel>(),
                // NewCatalogItems = itemsOnPage
                //     .Where(x => x.IsNew)
                //     .Take(12)
                //     .Select(i => new CatalogItemViewModel()
                //     {
                //         CatalogItemId = i.Id,
                //         CatalogItemName = i.Name,
                //         PictureUri = i.PictureUri,
                //         PictureHighUri = i.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri,
                //         Price = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) - i.Discount.Value : (i.Price ?? i.CatalogType.Price),
                //         PriceBeforeDiscount = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) : default(decimal?),
                //         ProductSlug = i.Slug,
                //         IsUnavailable = i.IsUnavailable
                //     }),
                FeaturedCatalogItems = itemsOnPage
                    .Where(x => x.IsFeatured)
                    .Take(24)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        PictureHighUri = i.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri,
                        Price = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) - i.Discount.Value : (i.Price ?? i.CatalogType.Price),
                        PriceBeforeDiscount = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) : default(decimal?),
                        ProductSlug = i.Slug,
                        IsUnavailable = i.IsUnavailable
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
            var illustrations = await _illustrationRepository.ListAsync();

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
            var types = await _typeRepository.ListAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "Todos", Selected = true }
            };
            foreach (CatalogType type in types)
            {
                items.Add(new SelectListItem() { Value = type.Id.ToString(), Text = type.Name });
            }

            return items;
        }

        public async Task<CategoryViewModel> GetCategoryCatalogItems(string categorySlug, int pageIndex, int? itemsPage, string onlyAvailable = null)
        {
            Category category = await GetCategoryFromUrl(categorySlug);
            if (category == null)
                return null;

            var filterSpecification = new CatalogFilterSpecification(null, null, category.Id, showOnlyAvailable: onlyAvailable == "true");
            var root = await _itemRepository
                .ListAsync(filterSpecification);

            var totalItems = root.Count();

            var iPage = itemsPage ?? totalItems;

            var allItems = root
                .OrderByDescending(x => x.Id)
                .ToList();

            //Get Catalog Types of Category
            var types = new List<CatalogType>();
            foreach (var item in allItems)
            {
                if (item.CatalogType.CatalogItems
                    .Where(x => x.Categories.Any(c => c.CategoryId == category.Id))
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

            if (allItems?.Count > 0)
            {
                var vm = new CatalogIndexViewModel()
                {
                    NewCatalogItems = allItems
                    .Where(x => x.IsNew)
                    .Take(12)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        PictureHighUri = i.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri,
                        Price = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) - i.Discount.Value : (i.Price ?? i.CatalogType.Price),
                        PriceBeforeDiscount = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) : default(decimal?),
                        ProductSlug = i.Slug,
                        IsUnavailable = i.IsUnavailable
                    }),
                    FeaturedCatalogItems = allItems
                    .Where(x => x.IsFeatured)
                    .Take(12)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        PictureHighUri = i.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri,
                        Price = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) - i.Discount.Value : (i.Price ?? i.CatalogType.Price),
                        PriceBeforeDiscount = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) : default(decimal?),
                        ProductSlug = i.Slug,
                        IsUnavailable = i.IsUnavailable
                    }),
                    CatalogTypes = types.OrderBy(x => x.Name).Select(x => new CatalogTypeViewModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
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
                        PictureHighUri = i.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri,
                        Price = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) - i.Discount.Value : (i.Price ?? i.CatalogType.Price),
                        PriceBeforeDiscount = i.Discount.HasValue ? (i.Price ?? i.CatalogType.Price) : default(decimal?),
                        ProductSlug = i.Slug,
                        IsUnavailable = i.IsUnavailable
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
                    Title = string.IsNullOrEmpty(category.Title) ? category.Name : category.Title,
                    DescriptionSection = new DescriptionViewModel
                    {
                        H1Text = category.H1Text,
                        Description = category.Description,
                        Question = $"Queres um {GetSingularyName(category.Name)} único e especial?"
                    }
                };
            }
            return null;
        }


        public async Task<ProductViewModel> GetCatalogItem(string slug)
        {
            var spec = new CatalogFilterBySlugSpecification(slug);
            var product = await _itemRepository.GetBySpecAsync(spec);

            if (product != null)
            {
                List<ProductReferenceViewModel> productReferences = new List<ProductReferenceViewModel>();
                if (product.References?.Count > 0)
                {
                    productReferences.Add(new ProductReferenceViewModel { Label = product.References.First().LabelDescription, Name = product.Name, Slug = product.Slug });
                    productReferences.AddRange(product.References
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
                    CategoryId = product.Categories.FirstOrDefault().CategoryId,
                    ProductTypeId = product.CatalogTypeId,
                    ProductSKU = product.Sku,
                    ProductTitle = product.Name,
                    ProductDescription = product.Description,
                    ProductPrice = product.Discount.HasValue ? (product.Price ?? product.CatalogType.Price) - product.Discount.Value : (product.Price ?? product.CatalogType.Price),
                    ProductPriceBeforeDiscount = product.Discount.HasValue ? (product.Price ?? product.CatalogType.Price) : default(decimal?),
                    ProductQuantity = 1,
                    ProductImagesUri = new List<(string, string)>
                    {
                        (product.PictureUri,product.Pictures?.SingleOrDefault(x => x.IsMain)?.PictureHighUri)
                    },
                    Attributes = new List<ProductAttributeViewModel>(),
                    Categories = product.Categories.Select(x => new LinkViewModel
                    {
                        Name = x.Category.Name,
                        TagName = x.Category.Slug
                    }).ToList(),
                    Tags = new List<LinkViewModel>
                    {
                        new LinkViewModel { Name = product.CatalogType.Name, Uri = product.CatalogType.Slug, TagName = "tipo"},
                        new LinkViewModel { Name = product.CatalogIllustration.Name, Uri = Uri.EscapeDataString(product.CatalogIllustration.Name), TagName = "ilustracao"},
                        new LinkViewModel { Name = product.CatalogIllustration.IllustrationType.Name, Uri = Uri.EscapeDataString(product.CatalogIllustration.IllustrationType.Name), TagName = "ilustracao_tipo"},
                    },
                    DeliveryTimeMin = product.CatalogType.DeliveryTimeMin,
                    DeliveryTimeMax = product.CatalogType.DeliveryTimeMax,
                    DeliveryTimeUnit = product.CatalogType.DeliveryTimeUnit,
                    CanCustomizeTotal = product.CanCustomize,
                    CustomizePrice = product.CatalogType.AdditionalTextPrice,
                    FirstCategoryId = product.Categories.FirstOrDefault()?.CategoryId ?? 0,
                    ProductReferences = productReferences,
                    PictureHelpers = product.CatalogType.PictureTextHelpers.Select(x => new PictureHelperViewModel
                    {
                        PictureUri = x.PictureUri,
                        PictureFileName = x.FileName
                    }).ToList(),
                    MetaDescription = !string.IsNullOrEmpty(product.MetaDescription) ?
                        product.MetaDescription :
                        !string.IsNullOrEmpty(product.CatalogType.MetaDescription) ?
                            $"{product.CatalogType.MetaDescription} {product.Name}" :
                            "",
                    Title = string.IsNullOrEmpty(product.Title) ? product.Name : product.Title,
                    IsUnavailable = product.IsUnavailable
                };

                //Others prictures
                if (product.Pictures.Where(x => x.IsActive && !x.IsMain).Count() > 0)
                    vm.ProductImagesUri.AddRange(
                        product.Pictures
                        .Where(x => x.IsActive && !x.IsMain)
                        .OrderBy(x => x.Order)
                        .Select(x => (x.PictureUri, x.PictureHighUri))
                        );

                //Attributes
                foreach (var grpAttr in product.Attributes.GroupBy(x => x.Type))
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

        private string GetSingularyName(string name)
        {
            if (name.ToLower().Last() == 's')
                return name.Substring(0, name.Length - 1);
            return name;
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

        public async Task<CatalogIndexViewModel> GetCatalogItemsByTag(int pageIndex, int? itemsPage, string tagName, TagType? tagType, string onlyAvailable = null)
        {
            //TODO: Add Count to EfRepository and get paging from DB, not in memory
            tagName = tagName.ToLower().Trim();
            var spec = new CatalogTagSpecification(tagName, tagType, onlyAvailable == "true");

            var allItems = await _itemRepository.ListAsync(spec);
            var totalItems = allItems.Count();
            if (totalItems == 0)
                return null;
            var iPage = itemsPage ?? totalItems;
            var itemsOnPage = allItems
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToList();

            var vm = new CatalogIndexViewModel
            {
                CatalogItems = itemsOnPage.Select(x => new CatalogItemViewModel
                {
                    CatalogItemId = x.Id,
                    CatalogItemName = x.Name,
                    PictureUri = x.PictureUri,
                    PictureHighUri = x.Pictures?.SingleOrDefault(p => p.IsMain)?.PictureHighUri,
                    Price = x.Discount.HasValue ? (x.Price ?? x.CatalogType.Price) - x.Discount.Value : (x.Price ?? x.CatalogType.Price),
                    PriceBeforeDiscount = x.Discount.HasValue ? (x.Price ?? x.CatalogType.Price) : default(decimal?),
                    ProductSlug = x.Slug,
                    IsUnavailable = x.IsUnavailable
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
        public async Task<CatalogIndexViewModel> GetCatalogItemsBySearch(int pageIndex, int? itemsPage, string searchFor, string onlyAvailable = null)
        {
            searchFor = searchFor.ToLower().Trim();

            var spec = new CatalogSearchSpecification(searchFor, onlyAvailable == "true");
            var items = await _itemRepository.ListAsync(spec);

            var totalItems = items.Count();
            var iPage = itemsPage ?? totalItems;
            var itemsOnPage = items
                .Skip(iPage * pageIndex)
                .Take(iPage)
                .ToList();


            var vm = new CatalogIndexViewModel
            {
                CatalogItems = itemsOnPage.Select(x => new CatalogItemViewModel
                {
                    CatalogItemId = x.Id,
                    CatalogItemName = x.Name,
                    PictureUri = x.PictureUri,
                    PictureHighUri = x.Pictures?.SingleOrDefault(p => p.IsMain)?.PictureHighUri,
                    Price = x.Price ?? x.CatalogType.Price,
                    ProductSlug = x.Slug,
                    IsUnavailable = x.IsUnavailable
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
            var spec = new CategorySpecification(true);
            var categories = await _categoryRepository.ListAsync(spec);

            List<MenuItemComponentViewModel> menuViewModel = new List<MenuItemComponentViewModel>();

            var parents = categories
                .Where(x => !x.ParentId.HasValue)
                .OrderBy(x => x.Order)
                .ToList();

            await GetTopCategoriesAsync(menuViewModel, categories, parents);

            await GetIllustrationsAsync(menuViewModel);


            return menuViewModel;
        }

        private async Task GetIllustrationsAsync(List<MenuItemComponentViewModel> menuViewModel)
        {
            var illustrations = await _illustrationRepository.ListAsync(new CatalogIllustrationSpecification(true));

            if (illustrations.Any())
            {
                var basePath = _environment.IsProduction() ? "/loja" : "";
                menuViewModel.Add(new MenuItemComponentViewModel
                {
                    Name = "COLECÇÔES",
                    Childs = illustrations.Select(x => new MenuItemComponentViewModel
                    {
                        Name = x.Name,
                        NameUri = $"{basePath}/tag/{x.Name.Replace(" ", "%2520").Replace("#", "%2523")}",
                        //NameUri = $"{basePath}/tag/{Uri.EscapeDataString(x.Name)}",
                        IsTag = true
                    }).ToList()
                });

            }

        }

        public async Task<CatalogTypeViewModel> GetCatalogTypeItemsAsync(string cat, string type, int pageIndex, int itemsPage, string onlyAvailable = null)
        {
            var category = await GetCategoryFromUrl(cat);

            CatalogType catalogType = await GetCatalogTypeFromUrl(type);

            if (catalogType == null || category == null)
                return null;

            return new CatalogTypeViewModel
            {
                CatNameUri = category.Slug,
                CategoryName = category.Name,
                Name = catalogType.Name,
                CatalogModel = await GetCatalogItems(pageIndex, itemsPage, null, catalogType.Id, category.Id, onlyAvailable),
                MetaDescription = catalogType.MetaDescription,
                Title = string.IsNullOrEmpty(catalogType.Title) ? catalogType.Name : catalogType.Title,
                DescriptionSection = new DescriptionViewModel
                {
                    H1Text = catalogType.H1Text,
                    Description = catalogType.Description,
                    Question = catalogType.Question
                }
            };
        }
        public async Task<string> GetSlugFromSkuAsync(string sku)
        {
            var spec = new CatalogSkuSpecification(sku);
            var catalogItem = await _itemRepository.GetBySpecAsync(spec);
            if (catalogItem != null)
                return catalogItem.Slug;
            return string.Empty;
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
                        item.Childs.AddRange(types.OrderBy(x => x.Name).Select(x => new MenuItemComponentViewModel
                        {
                            Id = x.Id,
                            Name = x.Name.ToUpper(),
                            NameUri = item.NameUri,
                            TypeUri = x.Slug
                        }));
                    }
                }
            }
        }


        private async Task<Category> GetCategoryFromUrl(string categorySlug)
        {
            var spec = new CategoryBySlugSpecification(categorySlug.ToLower());
            return await _categoryRepository.GetBySpecAsync(spec);
        }

        private async Task<CatalogType> GetCatalogTypeFromUrl(string type)
        {
            var spec = new CatalogTypeBySlugSpecification(type.ToLower());
            return await _typeRepository.GetBySpecAsync(spec);
        }
    }
}
