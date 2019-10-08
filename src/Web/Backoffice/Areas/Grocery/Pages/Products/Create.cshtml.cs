using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using ApplicationCore;

namespace Backoffice.Areas.Grocery.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public CreateModel(Infrastructure.Data.GroceryContext context, IBackofficeService service, IOptions<BackofficeSettings> backofficeSettings)
        {
            _context = context;
            _service = service;
            _backofficeSettings = backofficeSettings.Value;
        }

        [BindProperty]
        public CatalogItem CatalogItem { get; set; }

        [BindProperty]
        public IFormFile Picture { get; set; }

        [BindProperty]
        public List<CatalogCategoryViewModel> CatalogCategoryModel { get; set; } = new List<CatalogCategoryViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            CatalogItem = new CatalogItem
            {
                ShowOnShop = true
            };
            await PopulateListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateListAsync();
                return Page();
            }

            if(!CatalogCategoryModel.Any(x => x.Selected))
            {
                await PopulateListAsync();
                ModelState.AddModelError("", "Selecciona pelo menos uma categoria");
                return Page();
            }

            CatalogItem.Sku = _context.CatalogTypes.Find(CatalogItem.CatalogTypeId).Code;
            //Save Main Image            
            if (Picture?.Length > 0)
            {
                var lastCatalogItemId = (await _context.CatalogItems.AnyAsync()) ? (await _context.CatalogItems.LastAsync()).Id : 0;

                CatalogItem.PictureUri = _service.SaveFile(Picture, _backofficeSettings.GroceryProductsPictureFullPath, _backofficeSettings.GroceryProductsPictureUri, (++lastCatalogItemId).ToString(), true, 500, 500).PictureUri;
            }

            //Catalog Catagories
            CatalogItem.CatalogCategories = new List<CatalogCategory>();
            foreach (var item in CatalogCategoryModel.Where(x => x.Selected).ToList())
            {
                CatalogItem.CatalogCategories.Add(new CatalogCategory
                {
                    CategoryId = item.CategoryId
                });
                foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                {
                    CatalogItem.CatalogCategories.Add(new CatalogCategory
                    {
                        CategoryId = child.CategoryId
                    });
                }
            }

            _context.CatalogItems.Add(CatalogItem);
            await _context.SaveChangesAsync();

            //Update SKU
            CatalogItem.Sku += "-" + CatalogItem.Id;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task PopulateListAsync()
        {
            ViewData["CatalogTypeId"] = new SelectList(_context.CatalogTypes.Select(x => new { x.Id, Text = $"{x.Code} - {x.Name}" }), "Id", "Text");
            await SetCatalogCategoryModel();
        }

        private async Task SetCatalogCategoryModel()
        {
            //Catalog Categories      
            CatalogCategoryModel.Clear();
            var allCats = await _context.Categories
                .Include(x => x.Parent)
                .ToListAsync();

            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                CatalogCategoryViewModel parent = new CatalogCategoryViewModel
                {
                    CategoryId = item.Id,
                    Label = item.Name,
                    Childs = new List<CatalogCategoryViewModel>()

                };
                parent.Childs.AddRange(allCats.Where(x => x.ParentId == item.Id).Select(s => new CatalogCategoryViewModel
                {
                    CategoryId = s.Id,
                    Label = s.Name
                }));
                CatalogCategoryModel.Add(parent);
            }
        }
    }
}
