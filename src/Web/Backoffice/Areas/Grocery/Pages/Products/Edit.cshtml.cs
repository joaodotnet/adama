using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Backoffice.ViewModels;
using Microsoft.Extensions.Options;
using ApplicationCore;
using Backoffice.Interfaces;
using Backoffice.Pages;

namespace Backoffice.Areas.Grocery.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.GroceryContext _context;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public EditModel(Infrastructure.Data.GroceryContext context, IOptions<BackofficeSettings> settings, IBackofficeService service)
        {
            _context = context;
            _service = service;
            _backofficeSettings = settings.Value;
        }

        [BindProperty]
        public CatalogItem CatalogItem { get; set; }

        [BindProperty]
        public IFormFile Picture { get; set; }

        [BindProperty]
        public List<CatalogCategoryViewModel> CatalogCategoryModel { get; set; } = new List<CatalogCategoryViewModel>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CatalogItem = await _context.CatalogItems
                .Include(c => c.CatalogType)
                .Include(c => c.CatalogCategories)
                    .ThenInclude(cc => cc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (CatalogItem == null)
            {
                return NotFound();
            }

            await PopulateLists();
            return Page();
        }        

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateLists();
                return Page();
            }

            if (!CatalogCategoryModel.Any(x => x.Selected))
            {
                await PopulateLists();
                ModelState.AddModelError("", "Selecciona pelo menos uma categoria");
                return Page();
            }

            //Sku
            CatalogItem.Sku = $"{_context.CatalogTypes.Find(CatalogItem.CatalogTypeId).Code}-{CatalogItem.Id}";

            //Save Image
            if(Picture?.Length > 0)
            {
                if (!string.IsNullOrEmpty(CatalogItem.PictureUri))
                {
                    _service.DeleteFile(_backofficeSettings.GroceryProductsPictureFullPath, Utils.GetFileName(CatalogItem.PictureUri));
                }
                CatalogItem.PictureUri = _service.SaveFile(Picture, _backofficeSettings.GroceryProductsPictureFullPath, _backofficeSettings.GroceryProductsPictureUri, CatalogItem.Id.ToString(), true, 500, 500).PictureUri;
            }

            //CatalogCategories
            var catalogCategoriesDb = await _context.CatalogCategories
                .Include(x => x.Category)
                .Where(x => x.CatalogItemId == CatalogItem.Id)
                .ToListAsync();
            //Novos
            foreach (var item in CatalogCategoryModel.Where(x => x.Selected).ToList())
            {
                if (catalogCategoriesDb == null || !catalogCategoriesDb.Any(x => x.CategoryId == item.CategoryId))
                {
                    CatalogItem.CatalogCategories.Add(new CatalogCategory
                    {
                        CategoryId = item.CategoryId
                    });
                }
                foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                {
                    if (catalogCategoriesDb == null || !catalogCategoriesDb.Any(x => x.CategoryId == child.CategoryId))
                    {
                        CatalogItem.CatalogCategories.Add(new CatalogCategory
                        {
                            CategoryId = child.CategoryId
                        });
                    }
                }
            }
            //Remover
            foreach (var item in CatalogCategoryModel.ToList())
            {
                if (!item.Selected)
                {
                    var catalogCategory = catalogCategoriesDb.SingleOrDefault(x => x.CategoryId == item.CategoryId);
                    if(catalogCategory != null)
                    {
                        _context.Entry(catalogCategory).State = EntityState.Deleted;
                    }
                }
                    

                foreach (var child in item.Childs.Where(x => !x.Selected).ToList())
                {
                    var catalogCategory = catalogCategoriesDb.SingleOrDefault(x => x.CategoryId == child.CategoryId);
                    if (catalogCategory != null)
                    {
                        _context.Entry(catalogCategory).State = EntityState.Deleted;
                    }
                }

            }

            _context.Attach(CatalogItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatalogItemExists(CatalogItem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CatalogItemExists(int id)
        {
            return _context.CatalogItems.Any(e => e.Id == id);
        }

        private async Task PopulateLists()
        {
            ViewData["CatalogTypeId"] = new SelectList(_context.CatalogTypes.Select(x => new { x.Id, Text = $"{x.Code} - {x.Description}" }), "Id", "Text");
            await SetCatalogCategoryModel();
        }

        private async Task SetCatalogCategoryModel()
        {
            //Catalog Categories            
            var allCats = await _context.Categories
                .Include(x => x.Parent)
                .ToListAsync();
            CatalogCategoryModel.Clear();
            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                var catalogCategory = CatalogItem.CatalogCategories.SingleOrDefault(x => x.CategoryId == item.Id);
                CatalogCategoryViewModel parent = new CatalogCategoryViewModel
                {
                    Id = catalogCategory?.Id ?? 0,
                    CategoryId = item.Id,
                    Label = item.Name,
                    Selected = catalogCategory != null ? true : false,
                    Childs = new List<CatalogCategoryViewModel>()
                };
                parent.Childs.AddRange(allCats.Where(x => x.ParentId == item.Id).Select(s => new CatalogCategoryViewModel
                {
                    Id = CatalogItem.CatalogCategories.SingleOrDefault(x => x.CategoryId == s.Id)?.Id ?? 0,
                    CategoryId = s.Id,
                    Label = s.Name,
                    Selected = CatalogItem.CatalogCategories.Any(x => x.CategoryId == s.Id),
                }));
                CatalogCategoryModel.Add(parent);
            }
        }
    }
}
