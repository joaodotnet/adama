using ApplicationCore.Entities;
using DamaShopWeb.Web.Interfaces;
using DamaShopWeb.Web.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaShopWeb.Web.Services
{
    public class ShopService : IShopService
    {
        private readonly DamaContext _db;
        public ShopService(DamaContext db)
        {
            _db = db;
        }
        public async Task<MenuComponentViewModel> GetMenuList()
        {
            //TODO GET CACHE
            var categories = await _db.Categories
                .Include(x => x.Parent)
                .ToListAsync();

            MenuComponentViewModel menuViewModel = new MenuComponentViewModel();

            var parentsLeft = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "left")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(menuViewModel.Left, categories, parentsLeft);

            return menuViewModel;
        }

        private void GetTopCategories(List<MenuItemComponentViewModel> model, List<Category> categories, List<Category> parents)
        {
            model.AddRange(parents.Select(x => new MenuItemComponentViewModel { Id = x.Id, Name = x.Name }));

            //SubCategories
            foreach (var item in model)
            {
                var childs = categories
                    .Where(x => x.ParentId == item.Id)
                    .OrderBy(x => x.Order)
                    .ToList();
                item.Childs.AddRange(childs.Select(x => new MenuItemComponentViewModel { Id = x.Id, Name = x.Name }));
            }
        }
    }
}
