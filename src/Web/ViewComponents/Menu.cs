using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaShopWeb.Web.ViewComponents
{
    public class Menu : ViewComponent
    {
        private readonly DamaContext _db;
        public Menu(DamaContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new MenuComponentViewModel();

            //TODO GET CACHE
            var categories = await _db.Categories
                .Include(x => x.Parent)
                .ToListAsync();

            var parentsLeft = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "left")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(model.Left, categories, parentsLeft);

            var parentsRight = categories
                .Where(x => !x.ParentId.HasValue && x.Position == "right")
                .OrderBy(x => x.Order)
                .ToList();

            GetTopCategories(model.Right, categories, parentsRight);

            return View(model);
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

        public class MenuComponentViewModel
        {
            public List<MenuItemComponentViewModel> Left { get; set; } = new List<MenuItemComponentViewModel>();
            public List<MenuItemComponentViewModel> Right { get; set; } = new List<MenuItemComponentViewModel>();
        }

        public class MenuItemComponentViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<MenuItemComponentViewModel> Childs { get; set; } = new List<MenuItemComponentViewModel>();
        }
    }
}
