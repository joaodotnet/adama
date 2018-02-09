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

namespace Backoffice.Pages.ProductType
{
    public class CreateModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public CreateModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public ProductTypeViewModel ProductTypeModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
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