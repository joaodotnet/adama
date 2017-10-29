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
using Backoffice.RazorPages.ViewModels;

namespace Backoffice.RazorPages.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public EditModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                await _context.Products
                .Include(p => p.Illustation)
                .Include(p => p.ProductType)
                .SingleOrDefaultAsync(m => m.Id == id));

            if (ProductModel == null)
            {
                return NotFound();
            }
           ViewData["IllustrationId"] = new SelectList(_context.Illustrations, "Id", "Code");
           ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Code");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(_mapper.Map<Product>(ProductModel)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return RedirectToPage("./Index");
        }
    }
}
