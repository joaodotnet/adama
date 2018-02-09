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
                    Id = i.Id,
                    Name = i.Name,
                    PictureUri = i.PictureUri,
                    Price = i.Price
                }),
                NewCatalogItems = itemsOnPage
                    .Where(x => x.IsNew)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price
                    }),
                FeaturedCatalogItems = itemsOnPage
                    .Where(x => x.IsFeatured)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price
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
                .Where(x => x.CategoryId == categoryId)
                .Select(x => x.CatalogTypeId)
                .ToListAsync();
            if (types?.Count > 0)
            {
                var items = await _db.CatalogItems
                    .Include(x => x.CatalogType)
                    .Where(x => types.Exists(t => t == x.CatalogTypeId))
                    .ToListAsync();

                var vm = new CatalogIndexViewModel()
                {                    
                    NewCatalogItems = items
                    .Where(x => x.IsNew)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price
                    }),
                    FeaturedCatalogItems = items
                    .Where(x => x.IsFeatured)
                    .Take(8)
                    .Select(i => new CatalogItemViewModel()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        PictureUri = i.PictureUri,
                        Price = i.Price
                    }),
                    CatalogTypes = items.Select(x => new CatalogTypeViewModel()
                    {
                        Id = x.CatalogType.Id,
                        Code = x.CatalogType.Code,
                        Name = x.CatalogType.Description
                    })
                    .Distinct()
                    .ToList()
            };
                return vm;
            }
            return null;
        }
    }
}
