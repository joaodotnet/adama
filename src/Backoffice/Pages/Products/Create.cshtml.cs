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
            {
                await PopulateLists();
                return Page();
            }

            ProductModel.Sku = await _service.GetSku(ProductModel.CatalogTypeId, ProductModel.CatalogIllustrationId);
            if(await _service.CheckIfSkuExists(ProductModel.Sku))
            {
                await PopulateLists();
                ModelState.AddModelError("", $"O produto {ProductModel.Sku} já existe!");
                return Page();
            }

            //Save Main Image
            if (ProductModel.Picture.Length > 0)
            {
                ProductModel.PictureUri = await _service.SaveFileAsync(ProductModel.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri);
            }

            //Save other images
            if (ProductModel.OtherPictures?.Count > 0)
            {
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

            if (ProductModel.Picture?.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem principal? O máximo é 2MB, obrigado! Ass.: O seu amor!");               
            }

            //check if file exits
            if(ProductModel.Picture != null && _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, ProductModel.Picture.GetFileName()))
            {
                ModelState.AddModelError("",$"O nome da imagem {ProductModel.Picture.GetFileName()} já existe, por favor escolha outro nome!");
            }

            if (ProductModel.OtherPictures?.Count > 0)
            {
                foreach (var item in ProductModel.OtherPictures)
                {
                    if (item.Length > 2097152)
                        ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");

                    //check if file exits
                    if (item != null && _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, item.GetFileName()))
                    {
                        ModelState.AddModelError("", $"O nome da imagem {item.GetFileName()} já existe, por favor escolha outro nome!");
                    }
                }
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
                .Select(s => new { s.Id, Name = $"{s.Code} - {s.IllustrationType.Code} - {s.Name}" })
                .OrderBy(x => x.Name)
                .ToListAsync();
            ViewData["IllustrationId"] = new SelectList(illustrations, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.CatalogTypes.Select(x => new { x.Id, Name = $"{x.Code} - {x.Description}" }), "Id", "Name");
        }
    }
}