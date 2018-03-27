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
        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        [BindProperty]
        public List<CatalogCategoryViewModel> CatalogCategoryModel { get; set; } = new List<CatalogCategoryViewModel>();

        public async Task<IActionResult> OnGet()
        {
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

            if (!ValidatePictures())
            {
                await PopulateLists();
                return Page();
            }

            ProductModel.Sku = await _service.GetSku(ProductModel.CatalogTypeId, ProductModel.CatalogIllustrationId);

            //Save Main Image            
            if (ProductModel.Picture.Length > 0)
            {
                var lastCatalogItemId = (await _context.CatalogItems.LastAsync())?.Id ?? 0;

                ProductModel.PictureUri = await _service.SaveFileAsync(ProductModel.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri, (++lastCatalogItemId).ToString());
            }

            //Save other images
            if (ProductModel.OtherPictures?.Count > 0)
            {
                var lastCatalogPictureId = _context.CatalogPictures.Count() > 0 ? (await _context.CatalogPictures.LastAsync()).Id : 0;
                var order = 0;
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

            //Catalog Catagories
            var prod = _mapper.Map<CatalogItem>(ProductModel);            
            foreach (var item in CatalogCategoryModel.Where(x => x.Selected).ToList())
            {
                prod.CatalogCategories.Add(new CatalogCategory
                {
                    CategoryId = item.CategoryId
                });
                foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                {
                    prod.CatalogCategories.Add(new CatalogCategory
                    {
                        CategoryId = child.CategoryId
                    });
                }
            }

            //Save Changes                        
            _context.CatalogItems.Add(prod);
            await _context.SaveChangesAsync();

            //Update Sku
            prod.Sku += "_" + prod.Id;
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

            if (ProductModel.OtherPictures?.Count > 0)
            {
                foreach (var item in ProductModel.OtherPictures)
                {
                    if (item.Length > 2097152)
                        ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");
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
            var types = _context.CatalogTypes.Select(x => new { x.Id, Name = $"{x.Code} - {x.Description}" });
            ViewData["ProductTypeId"] = new SelectList(types, "Id", "Name");
            await SetCatalogCategoryModel(types.First().Id);
        }

        private async Task SetCatalogCategoryModel(int catalogTypeId)
        {
            //Catalog Categories            
            var allCats = await _context.Categories
                .Include(x => x.Parent)
                .Include(x => x.CatalogTypes)
                .ToListAsync();

            var catsId = allCats
                .Where(x => x.CatalogTypes.Any(ct => ct.CatalogTypeId == catalogTypeId))
                .Select(x => x.Id)
                .ToList();

            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                CatalogCategoryViewModel parent = new CatalogCategoryViewModel
                {
                    CategoryId = item.Id,
                    Label = item.Name,
                    Childs = new List<CatalogCategoryViewModel>(),
                    Selected = catsId.Contains(item.Id)

                };
                parent.Childs.AddRange(allCats.Where(x => x.ParentId == item.Id).Select(s => new CatalogCategoryViewModel
                {
                    CategoryId = s.Id,
                    Label = s.Name,
                    Selected = catsId.Contains(s.Id)
                }));
                CatalogCategoryModel.Add(parent);
            }
        }
    }
}