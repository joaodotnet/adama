using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.ViewModels;
using Microsoft.EntityFrameworkCore;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using Backoffice.Extensions;

namespace Backoffice.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public CreateModel(DamaContext context, IMapper mapper, IBackofficeService service, IOptions<BackofficeSettings> backofficeSettings)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = backofficeSettings.Value;
        }

        public async Task<IActionResult> OnGet()
        {
            await PopulateLists();
            return Page();
        }        

        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateLists();
                return Page();
            }

            if (!ValidatePictures())
                return Page();

            //Save Main Image
            if (ProductModel.Picture.Length > 0)
            {
                ProductModel.PictureUri = await _service.SaveFileAsync(ProductModel.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri);
            }

            //Save other images
            var order = 0;
            foreach (var item in ProductModel.OtherPictures)
            {
                ProductModel.CatalogPictures.Add(new ProductPictureViewModel
                {
                    IsActive = true,
                    Order = ++order,
                    PictureUri = await _service.SaveFileAsync(item, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri)
                });
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
            }

            //Save Changes            
            _context.CatalogItems.Add(_mapper.Map<CatalogItem>(ProductModel));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAddAttributeAsync()
        {
            await PopulateLists();
            ProductModel.CatalogAttributes.Add(new ProductAttributeViewModel());
            return Page();
        }

        private bool ValidatePictures()
        {
            if (ProductModel.Picture == null || ProductModel.Picture.Length == 0)
            {
                ModelState.AddModelError("", "A menina quer por favor escolher uma imagem principal, obrigado! Ass.: O seu amor!");                
            }

            if (ProductModel.Picture.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem principal? O máximo é 2MB, obrigado! Ass.: O seu amor!");               
            }
            foreach (var item in ProductModel.OtherPictures)
            {
                if (item.Length > 2097152)
                    ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");
            }
            return ModelState.IsValid;
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
    }
}