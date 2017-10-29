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
using Backoffice.RazorPages.ViewModels;

namespace Backoffice.RazorPages.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public CreateModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult OnGet()
        {
        ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
        ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
            return Page();
        }

        [BindProperty]
        public ProductViewModel ProductModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
                ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
                return Page();
            }

            _context.Products.Add(_mapper.Map<Product>(ProductModel));
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}