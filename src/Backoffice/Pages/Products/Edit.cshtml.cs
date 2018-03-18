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
using AutoMapper;
using Backoffice.ViewModels;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using Backoffice.Extensions;

namespace Backoffice.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public EditModel(DamaContext context, IMapper mapper, IOptions<BackofficeSettings> settings, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = settings.Value;
        }

        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        [BindProperty]
        public List<CatalogCategoryViewModel> CatalogCategoryModel { get; set; } = new List<CatalogCategoryViewModel>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductModel = _mapper.Map<ProductViewModel>(
                await _context.CatalogItems
                .Include(p => p.CatalogCategories)
                .Include(p => p.CatalogIllustration)
                .Include(p => p.CatalogType)
                .Include(p => p.CatalogAttributes)
                .ThenInclude(ca => ca.ReferenceCatalogItem)
                .Include(p => p.CatalogPictures)
                .SingleOrDefaultAsync(m => m.Id == id));

            if (ProductModel == null)
            {
                return NotFound();
            }
            await PopulateLists();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateLists();
                return Page();
            }
           
            //Validade Pictures
            if (!ValidatePictures())
            {
                await PopulateLists();
                return Page();
            }

            //Validate SKU
            ProductModel.Sku = await _service.GetSku(ProductModel.CatalogTypeId, ProductModel.CatalogIllustrationId) + "_" + ProductModel.Id; 
            
            //Main Picture
            if (ProductModel.Picture != null && ProductModel.Picture.Length > 0)
            {
                if (!string.IsNullOrEmpty(ProductModel.PictureUri))
                {
                    _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(ProductModel.PictureUri));
                }
                ProductModel.PictureUri = await _service.SaveFileAsync(ProductModel.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri, ProductModel.Id.ToString());
            }

            //Update images            
            foreach (var item in ProductModel.CatalogPictures.Where(x => x.Picture != null && !x.ToRemove).ToList())
            {
                _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(item.PictureUri));
                item.PictureUri = await _service.SaveFileAsync(item.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri, item.Id.ToString());
            }
            //Save news images
            if (ProductModel.OtherPictures != null)
            {
                var order = ProductModel.CatalogPictures.Count == 0 ? 0 : ProductModel.CatalogPictures.Max(x => x.Order);
                var lastCatalogPictureId = _context.CatalogPictures.Count() > 0 ? (await _context.CatalogPictures.LastAsync()).Id : 0;
                foreach (var item in ProductModel.OtherPictures)
                {
                    ProductModel.CatalogPictures.Add(new ProductPictureViewModel
                    {
                        IsActive = true,
                        Order = ++order,
                        PictureUri = await _service.SaveFileAsync(item, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri, (++lastCatalogPictureId).ToString())
                    });
                }
            }

            //Save Changes            
            var prod = _mapper.Map<CatalogItem>(ProductModel);            
            //foreach (var item in prod.CatalogAttributes)
            //{
            //    if (item.Id != 0)
            //    {
            //        if (ProductModel.CatalogAttributes.SingleOrDefault(x => x.Id == item.Id).ToRemove)
            //            _context.Entry(item).State = EntityState.Deleted;
            //        else
            //            _context.Entry(item).State = EntityState.Modified;
            //    }
            //    else
            //        _context.Entry(item).State = EntityState.Added;
            //}
            foreach (var item in prod.CatalogPictures)
            {
                if (item.Id != 0)
                {
                    if (ProductModel.CatalogPictures.SingleOrDefault(x => x.Id == item.Id).ToRemove)
                    {
                        _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(item.PictureUri));
                        _context.Entry(item).State = EntityState.Deleted;
                    }
                    else
                        _context.Entry(item).State = EntityState.Modified;
                }
            }

            //Categorias dos Produtos
            var catalogCategoriesDb = await _context.CatalogCategories.Where(x => x.CatalogItemId == prod.Id).ToListAsync();
            //Novos
            foreach (var item in CatalogCategoryModel.Where(x => x.Selected).ToList())
            {
                if(catalogCategoriesDb == null || !catalogCategoriesDb.Any(x => x.CategoryId == item.CategoryId))
                {
                    prod.CatalogCategories.Add(new CatalogCategory
                    {                        
                        CategoryId = item.CategoryId
                    });
                }
                foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                {
                    if (catalogCategoriesDb == null || !catalogCategoriesDb.Any(x => x.CategoryId == child.CategoryId))
                    {
                        prod.CatalogCategories.Add(new CatalogCategory
                        {
                            CategoryId = child.CategoryId
                        });
                    }
                }
            }
            //Remover
            foreach (var item in CatalogCategoryModel.Where(x => x.Id != 0).ToList())
            {
                if (!item.Selected)
                    _context.CatalogCategories.Remove(_context.CatalogCategories.Find(item.Id));

                foreach (var child in item.Childs.Where(x => x.Id != 0 && !x.Selected).ToList())
                {
                    _context.CatalogCategories.Remove(_context.CatalogCategories.Find(child.Id));
                }

            }

            _context.Attach(prod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return RedirectToPage("./Index");
        }

        private bool ValidateAttributesModel()
        {
            foreach (var item in ProductModel.CatalogAttributes)
            {
                if (!item.ToRemove)
                {
                    //Validate
                    //if (string.IsNullOrEmpty(item.Code))
                    //    ModelState.AddModelError("", "O código do atributo é obrigatório");
                    if (string.IsNullOrEmpty(item.Name))
                        ModelState.AddModelError("", "O nome do atributo é obrigatório");
                    if (!ModelState.IsValid)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<IActionResult> OnPostAddAttributeAsync()
        {
            await PopulateLists();
            ProductModel.CatalogAttributes.Add(new ProductAttributeViewModel
            {
                CatalogItemId = ProductModel.Id
            });
            return Page();
        }

        private async Task PopulateLists()
        {
            var illustrations = await _context.CatalogIllustrations
                .Include(x => x.IllustrationType)
                .Select(s => new { s.Id, Name = $"{s.Code} - {s.IllustrationType.Code} - {s.Name}" })
                .OrderBy(x => x.Name)
                .ToListAsync();
            ViewData["IllustrationId"] = new SelectList(illustrations, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.CatalogTypes.Select(x => new { x.Id, Name = $"{x.Code} - {x.Description}" }), "Id", "Name");
            await SetCatalogCategoryModel();
        }

        private bool ValidatePictures()
        {
            if (string.IsNullOrEmpty(ProductModel.PictureUri) && ProductModel.Picture == null)
            {
                ModelState.AddModelError("", "A imagem principal é obrigatória!");
            }

            if (ProductModel.Picture != null && ProductModel.Picture.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem principal? O máximo é 2MB, obrigado! Ass.: O seu amor!");
            }

            //check if file exits but not the same
            //if (ProductModel.Picture != null && 
            //    ProductModel.Picture.GetFileName() != Utils.GetFileName(ProductModel.PictureUri) && 
            //    _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, ProductModel.Picture.GetFileName()))
            //{
            //    ModelState.AddModelError("", $"O nome da imagem {ProductModel.Picture.GetFileName()} já existe, por favor escolha outro nome!");
            //}
            
            foreach (var item in ProductModel.CatalogPictures)
            {
                if (item.Picture != null && item.Picture.Length > 2097152)
                    ModelState.AddModelError("", $"A imagem {item.Picture.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");

                //if(item.Picture != null && string.IsNullOrEmpty(item.PictureUri) && _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath,item.Picture.GetFileName()))
                //    ModelState.AddModelError("", $"O nome da imagem {item.Picture.GetFileName()} já existe, por favor escolha outro nome!");

                //if(item.Picture != null && 
                //    !string.IsNullOrEmpty(item.PictureUri) &&
                //    item.Picture.GetFileName() != Utils.GetFileName(item.PictureUri) &&
                //    _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, item.Picture.GetFileName()))
                //    ModelState.AddModelError("", $"O nome da imagem {item.Picture.GetFileName()} já existe, por favor escolha outro nome!");
            }

            if (ProductModel.OtherPictures != null)
            {
                foreach (var item in ProductModel.OtherPictures)
                {
                    if (item.Length > 2097152)
                        ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");

                    //if (item != null &&
                    //    _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, item.GetFileName()))
                    //    ModelState.AddModelError("", $"O nome da imagem {item.GetFileName()} já existe, por favor escolha outro nome!");
                }
            }

            return ModelState.IsValid;
        }

        private async Task SetCatalogCategoryModel()
        {
            //Catalog Categories            
            var allCats = await _context.Categories.Include(x => x.Parent).ToListAsync();
            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                var catalogCategory = ProductModel.CatalogCategories.SingleOrDefault(x => x.CategoryId == item.Id);
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
                    Id = ProductModel.CatalogCategories.SingleOrDefault(x => x.CategoryId == s.Id)?.Id ?? 0,
                    CategoryId = s.Id,
                    Label = s.Name,
                    Selected = ProductModel.CatalogCategories.Any(x => x.CategoryId == s.Id),
                }));
                CatalogCategoryModel.Add(parent);
            }
        }
    }
}
