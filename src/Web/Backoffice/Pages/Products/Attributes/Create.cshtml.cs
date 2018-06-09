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

namespace Backoffice.Pages.Products.Attributes
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public CreateModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var prod = await _context.CatalogItems.FindAsync(id);
            if (prod == null)
                return NotFound();

            CatalogAttributeModel.CatalogItemId = id;
            CatalogAttributeModel.CatalogItem = _mapper.Map<ProductViewModel>(prod);
            
            ViewData["ReferenceCatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public ProductAttributeViewModel CatalogAttributeModel { get; set; } = new ProductAttributeViewModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            CatalogAttributeModel.CatalogItem = null;
            _context.CatalogAttributes.Add(_mapper.Map<CatalogAttribute>(CatalogAttributeModel));
            await _context.SaveChangesAsync();

            return RedirectToPage("/Products/Edit", new { id = CatalogAttributeModel.CatalogItemId });
        }
    }
}