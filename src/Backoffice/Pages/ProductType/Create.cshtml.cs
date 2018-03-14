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
using AutoMapper;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Pages.ProductType
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public CreateModel(Infrastructure.Data.DamaContext context, IMapper mapper, IBackofficeService service, IOptions<BackofficeSettings> backofficeSettings)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = backofficeSettings.Value;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Units"] = new SelectList(new object[] { new { Id = "dias" }, new { Id = "semanas" }, new { Id = "meses" } }, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public ProductTypeViewModel ProductTypeModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Units"] = new SelectList(new object[] { new { Id = "dias" }, new { Id = "semanas" }, new { Id = "meses" } }, "Id", "Id");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //check if code exists
            if (_context.CatalogTypes.Any(x => x.Code.ToUpper() == ProductTypeModel.Code.ToUpper()))
            {
                ModelState.AddModelError("", $"O nome do Tipo do Produto '{ProductTypeModel.Code}' já existe!");
                return Page();
            }

            if(ProductTypeModel.CategoriesId == null || ProductTypeModel.CategoriesId.Count == 0)
            {
                ModelState.AddModelError("", "O campo Categorias é obrigatório");
                return Page();
            }

            if (ProductTypeModel.Picture?.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem? O máximo é 2MB, obrigado! Ass.: O seu amor!");
                return Page();
            }

            //Save Image
            if (ProductTypeModel?.Picture?.Length > 0)
            {
                var lastId = _context.CatalogTypes.Count() > 0 ? (await _context.CatalogTypes.LastAsync()).Id : 0;
                ProductTypeModel.PictureUri = await _service.SaveFileAsync(ProductTypeModel.Picture, _backofficeSettings.WebProductTypesPictureFullPath, _backofficeSettings.WebProductTypesPictureUri, (++lastId).ToString());
            }

            var catalogType = _mapper.Map<ApplicationCore.Entities.CatalogType>(ProductTypeModel);

            catalogType.Categories = new List<CatalogTypeCategory>();
            foreach (var item in ProductTypeModel.CategoriesId)
            {
                catalogType.Categories.Add(new CatalogTypeCategory
                {
                    CategoryId = item
                });
            }

            _context.CatalogTypes.Add(catalogType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}