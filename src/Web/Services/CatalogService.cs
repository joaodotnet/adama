using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Web.Interfaces;
using Web.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Web.Extensions;

namespace Web.Services
{
    /// <summary>
    /// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
    /// with UI-specific types (view models and SelectListItem types).
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ILogger<CatalogService> _logger;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IAsyncRepository<CatalogIllustration> _brandRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IUriComposer _uriComposer;
        private readonly DamaContext _db; //TODO move all queries to repo

        public CatalogService(
            ILoggerFactory loggerFactory,
            IRepository<CatalogItem> itemRepository,
            IAsyncRepository<CatalogIllustration> brandRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IUriComposer uriComposer,
            DamaContext db)
        {
            _logger = loggerFactory.CreateLogger<CatalogService>();
            _itemRepository = itemRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
            _uriComposer = uriComposer;
            _db = db;
        }

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? illustrationId, int? typeId)
        {
            _logger.LogInformation("GetCatalogItems called.");

            var filterSpecification = new CatalogFilterSpecification(illustrationId, typeId);
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
                    Price = i.Price,
                    ProductSku = i.Sku
                }),
                NewCatalogItems = itemsOnPage
                    .Where(x => x.IsNew)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price,
                        ProductSku = i.Sku,
                    }),
                FeaturedCatalogItems = itemsOnPage
                    .Where(x => x.IsFeatured)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price,
                        ProductSku = i.Sku,
                    }),
                Brands = await GetBrands(),
                Types = await GetTypes(),
                BrandFilterApplied = illustrationId ?? 0,
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

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            _logger.LogInformation("GetBrands called.");
            var brands = await _brandRepository.ListAllAsync();

            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            foreach (CatalogIllustration brand in brands)
            {
                items.Add(new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Code });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            _logger.LogInformation("GetTypes called.");
            var types = await _typeRepository.ListAllAsync();
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            foreach (CatalogType type in types)
            {
                items.Add(new SelectListItem() { Value = type.Id.ToString(), Text = type.Code });
            }

            return items;
        }

        public async Task<CatalogIndexViewModel> GetCategoryCatalogItems(int categoryId)
        {
            //TODO: Move to repo
            var types = await _db.CatalogTypeCategories
                .Include(x => x.Category)
                .Include(x => x.CatalogType)
                .Where(x => x.CategoryId == categoryId)
                //.Select(x => x.CatalogType)
                .ToListAsync();
            if (types?.Count > 0)
            {
                var items = await _db.CatalogItems
                    .Include(x => x.CatalogType)
                    .Where(x => types.Any(t => t.CatalogTypeId == x.CatalogTypeId))
                    .ToListAsync();

                var vm = new CatalogIndexViewModel()
                {
                    NewCatalogItems = items
                    .Where(x => x.IsNew)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price,
                        ProductSku = i.Sku
                    }),
                    FeaturedCatalogItems = items
                    .Where(x => x.IsFeatured)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        CatalogItemId = i.Id,
                        CatalogItemName = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price,
                        ProductSku = i.Sku
                    }),
                    CatalogTypes = types.Select(x => new CatalogTypeViewModel()
                    {
                        Id = x.CatalogType.Id,
                        Code = x.CatalogType.Code,
                        Name = x.CatalogType.Description,
                        PictureUri = x.CatalogType.PictureUri,
                        CatNameUri = Utils.StringToUri(x.Category.Name),
                        TypeNameUri = Utils.StringToUri(x.CatalogType.Description)
                    })
                    .Distinct()
                    .ToList()
                };
                return vm;
            }
            return null;
        }

        public async Task<ProductViewModel> GetCatalogItem(string sku)
        {
            var product = await _db.CatalogItems
                .Include(x => x.CatalogPictures)
                .Include(x => x.CatalogAttributes)                
                .Include(x => x.CatalogType)
                    .ThenInclude(ct => ct.Categories)
                        .ThenInclude(c => c.Category)
                .Include(x => x.CatalogIllustration)
                    .ThenInclude(ci => ci.IllustrationType)
                .SingleOrDefaultAsync(x => x.Sku == sku);

            if (product != null)
            {
                var vm = new ProductViewModel
                {
                    ProductId = product.Id,
                    ProductSKU = product.Sku,
                    ProductTitle = product.Name,
                    ProductDescription = product.Description,
                    ProductBasePrice = product.Price,
                    ProductQuantity = 1,
                    ProductImagesUri = new List<string>
                    {
                        product.PictureUri
                    },
                    Attributes = new List<ProductAttributeViewModel>(),
                    Categories = new List<LinkViewModel>(),
                    Tags = new List<LinkViewModel>
                    {
                        new LinkViewModel { Name = product.CatalogType.Description, Uri = "#"},
                        new LinkViewModel { Name = product.CatalogIllustration.Name, Uri = "#"},
                        new LinkViewModel { Name = product.CatalogIllustration.IllustrationType.Name, Uri = "#"},
                    }
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
                foreach(var grpAttr in product.CatalogAttributes.GroupBy(x => x.Type))
                {
                    //attrPriceDefault += grpAttr.First().Price ?? 0;
                    vm.Attributes.Add(new ProductAttributeViewModel
                    {
                        AttributeType = grpAttr.Key,
                        Items = new SelectList(grpAttr.ToList(), "Id", "Name"),
                        Label = EnumHelper<CatalogAttributeType>.GetDisplayValue(grpAttr.Key),
                        DefaultText = GetDefaultText(grpAttr.Key),
                        Selected = grpAttr.First().Id,
                        Attributes = grpAttr.Select(x => new AttributeViewModel
                        {
                            Id = x.Id,
                            Price = x.Price,
                            Sku = x.Sku
                        }).ToList()
                    });
                }
                //vm.ProductBasePrice += attrPriceDefault;
                //Categories
                foreach (var item in product.CatalogType.Categories)
                {
                    vm.Categories.Add(new LinkViewModel
                    {
                        Name = item.Category.Name,
                        Uri = $"/{Utils.StringToUri(item.Category.Name)}"
                    });
                }
                return vm;
            }
            return null;
        }

        private string GetDefaultText(CatalogAttributeType key)
        {
            switch (key)
            {
                case CatalogAttributeType.SIZE:
                    return "Escolha um tamanho";
                case CatalogAttributeType.BOOK_FORMAT:
                    return "Escolha um formato";
                default:
                    return null;
            }
        }

        public async Task<AttributeViewModel> GetAttributeDetails(int attributeId)
        {
            var attr = await _db.CatalogAttributes
                .Include(x => x.ReferenceCatalogItem)
                .SingleOrDefaultAsync(x => x.Id == attributeId);
            if (attr != null)
                return new AttributeViewModel
                {
                    Price = attr.Price,
                    ReferenceCatalogSku = attr.ReferenceCatalogItem?.Sku
                };
            return null;
        }
    }
}
