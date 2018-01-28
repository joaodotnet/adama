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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductModel = _mapper.Map<ProductViewModel>(
                await _context.CatalogItems
                .Include(p => p.CatalogIllustration)
                .Include(p => p.CatalogType)
                .Include(p => p.CatalogAttributes)
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
            //Remove model attributes with no id
            var to_remove = ProductModel.CatalogAttributes.Where(x => x.ToRemove && x.Id == 0).ToList();
            foreach (var item in to_remove)
            {
                ProductModel.CatalogAttributes.Remove(item);
            }
            //Validate Model
            if (!ValidateAttributesModel())
            {
                await PopulateLists();
                return Page();
            }

            if (!ValidatePictures())
            {
                await PopulateLists();
                return Page();
            }

            //Main Picture
            if (ProductModel.Picture != null && ProductModel.Picture.Length > 0)
            {
                if (!string.IsNullOrEmpty(ProductModel.PictureUri))
                    _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(ProductModel.PictureUri));
                ProductModel.PictureUri = await _service.SaveFileAsync(ProductModel.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri);
            }

            //Update images            
            foreach (var item in ProductModel.CatalogPictures.Where(x => x.Picture != null && !x.ToRemove).ToList())
            {
                _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(item.PictureUri));
                item.PictureUri = await _service.SaveFileAsync(item.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri);
            }
            //Save news images
            if (ProductModel.OtherPictures != null)
            {
                var order = ProductModel.CatalogPictures.Count == 0 ? 0 : ProductModel.CatalogPictures.Max(x => x.Order);
                foreach (var item in ProductModel.OtherPictures)
                {
                    ProductModel.CatalogPictures.Add(new ProductPictureViewModel
                    {
                        IsActive = true,
                        Order = ++order,
                        PictureUri = await _service.SaveFileAsync(item, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri)
                    });
                }
            }

            //Save Changes            
            var prod = _mapper.Map<CatalogItem>(ProductModel);
            foreach (var item in prod.CatalogAttributes)
            {
                if (item.Id != 0)
                {
                    if (ProductModel.CatalogAttributes.SingleOrDefault(x => x.Id == item.Id).ToRemove)
                        _context.Entry(item).State = EntityState.Deleted;
                    else
                        _context.Entry(item).State = EntityState.Modified;
                }
                else
                    _context.Entry(item).State = EntityState.Added;
            }
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
                    if (string.IsNullOrEmpty(item.Code))
                        ModelState.AddModelError("", "O código do atributo é obrigatório");
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
                .Select(s => new { Id = s.Id, Name = $"{s.IllustrationType.Code} - {s.Code}" })
                .OrderBy(x => x.Name)
                .ToListAsync();
            ViewData["IllustrationId"] = new SelectList(illustrations, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.CatalogTypes, "Id", "Code");
        }

        private bool ValidatePictures()
        {
            if (ProductModel.Picture != null && ProductModel.Picture.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem principal? O máximo é 2MB, obrigado! Ass.: O seu amor!");
            }

            if (ProductModel.OtherPictures != null)
            {
                foreach (var item in ProductModel.OtherPictures)
                {
                    if (item.Length > 2097152)
                        ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");
                }
            }
            foreach (var item in ProductModel.CatalogPictures)
            {
                if (item.Picture != null && item.Picture.Length > 2097152)
                    ModelState.AddModelError("", $"A imagem {item.Picture.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");
            }
            return ModelState.IsValid;
        }
    }
}
