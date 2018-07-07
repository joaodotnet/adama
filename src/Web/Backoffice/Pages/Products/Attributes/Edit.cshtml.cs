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
using Backoffice.Extensions;
using Backoffice.Interfaces;

namespace Backoffice.Pages.Products.Attributes
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;

        public EditModel(DamaContext context, IMapper mapper, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            this._service = service;
        }

        [BindProperty]
        public ProductAttributeViewModel CatalogAttributeModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ca = await _context.CatalogAttributes
                .Include(c => c.CatalogItem)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (ca == null)
            {
                return NotFound();
            }
            CatalogAttributeModel = _mapper.Map<ProductAttributeViewModel>(ca);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            CatalogAttributeModel.CatalogItem = null;
            var ca = _mapper.Map<CatalogAttribute>(CatalogAttributeModel);
            _context.Attach(ca).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }
            return RedirectToPage("/Products/Edit", new { id = ca.CatalogItemId });
        }
    }
}
