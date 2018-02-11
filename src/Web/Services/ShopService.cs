using ApplicationCore.Entities;
using AutoMapper;
using Web.Interfaces;
using Web.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamaShopWeb.Web;
using System.IO;

namespace Web.Services
{
    public class ShopService : IShopService
    {
        private readonly DamaContext _db;
        private readonly IMapper _mapper;
        public ShopService(DamaContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CatalogType> GetCatalogType(string type)
        {
            var allCatalogTypes = await _db.CatalogTypes.ToListAsync();
            foreach (var item in allCatalogTypes)
            {
                var typeName = item.Description.Replace(" ", "-").ToLower();
                if (Utils.RemoveDiacritics(typeName) == type)
                    return item;
            }
            return null;
        }

        public async Task<Category> GetCategory(string name)
        {
            var allCategories = await _db.Categories.ToListAsync();
            foreach (var item in allCategories)
            {
                var catName = item.Name.Replace(" ", "-").ToLower();
                if (Utils.RemoveDiacritics(catName) == name.ToLower())
                    return item;
            }
            return null;
        }

        public async Task<List<MainBannerViewModel>> GetMainBanners()
        {
            var bannerConfig = await _db.ShopConfigDetails
                .Include(x => x.ShopConfig)
                .Where(x => x.ShopConfig.Type == ShopConfigType.NEWS_BANNER && x.ShopConfig.IsActive && x.IsActive)
                .ToListAsync();
            return _mapper.Map<List<MainBannerViewModel>>(bannerConfig);
        }

        public async Task<MenuComponentViewModel> GetMenuList()
        {
            //TODO GET CACHE
            var categories = await _db.Categories
                .Include(x => x.Parent)
                .Include(x => x.CatalogTypes)
                .ThenInclude(ct => ct.CatalogType)
                .ToListAsync();

            MenuComponentViewModel menuViewModel = new MenuComponentViewModel();

            var parentsLeft = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "left")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(menuViewModel.Left, categories, parentsLeft);

            var parentsRight = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "right")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(menuViewModel.Right, categories, parentsRight);

            return menuViewModel;
        }

        private void GetTopCategories(List<MenuItemComponentViewModel> model, List<Category> categories, List<Category> parents)
        {
            model.AddRange(parents.Select(x => new MenuItemComponentViewModel
            {
                Id = x.Id,
                Name = x.Name.ToUpper(),
                MenuUri = "/" + Utils.RemoveDiacritics(x.Name).Replace(" ", "-").ToLower()
            }));

            //SubCategories
            foreach (var item in model)
            {
                var childs = categories
                    .Where(x => x.ParentId == item.Id)
                    .OrderBy(x => x.Order)
                    .ToList();

                if(childs?.Count > 0)
                {
                    item.Childs.AddRange(childs.Select(x => new MenuItemComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name.ToUpper(),
                        MenuUri = "/" + Utils.RemoveDiacritics(x.Name).Replace(" ", "-").ToLower()
                    }));
                }
                else
                {
                    var types = categories
                        .Where(x => x.Id == item.Id)
                        .Select(x => x.CatalogTypes);

                    var catalogTypes = types.SelectMany(x => x.Select(t => t.CatalogType));

                    item.Childs.AddRange(catalogTypes.Select(x => new MenuItemComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Description.ToUpper(),
                        MenuUri = "/" + Path.Combine(
                            Utils.RemoveDiacritics(item.Name).Replace(" ", "-").ToLower(),
                            Utils.RemoveDiacritics(x.Description).Replace(" ", "-").ToLower())
                    }));
                }
            }
        }
    }
}
