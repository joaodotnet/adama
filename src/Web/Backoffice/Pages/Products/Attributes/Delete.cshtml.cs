using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.ViewModels;
using Backoffice.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Backoffice.Pages.Products.Attributes
{
    public class DeleteModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;

        public DeleteModel(DamaContext context, IMapper mapper, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
        }

        [BindProperty]
        public AttributeViewModel CatalogAttributeModel { get; set; }

        public string ProductName { get; set; }

        public class AttributeViewModel
        {
            public int Id { get; set; }
            public AttributeType Type { get; set; }
            [Display(Name = "Nome")]
            [Required]
            [StringLength(100)]
            public string Name { get; set; }
            public int CatalogItemId { get; set; }
        }

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

            CatalogAttributeModel = _mapper.Map<AttributeViewModel>(ca);
            ProductName = ca.Name;    
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ca = await _context.CatalogAttributes.FindAsync(id);

            if (ca != null)
            {
                _context.CatalogAttributes.Remove(ca);
                await _context.SaveChangesAsync();

                
                //await _service.DeleteCatalogPrice(ca.CatalogItemId, ca.Id);
            }

            return RedirectToPage("/Products/Edit", new { id = ca.CatalogItemId });
        }
    }
}
