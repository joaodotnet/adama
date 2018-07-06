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

namespace Backoffice.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public DetailsModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ProductViewModel ProductModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductModel = _mapper.Map<ProductViewModel>(await _context.CatalogItems
                .Include(p => p.CatalogIllustration)
                    .ThenInclude(i => i.IllustrationType)
                .Include(p => p.CatalogType)
                .Include(p => p.CatalogAttributes)
                .ThenInclude(ca => ca.ReferenceCatalogItem)
                .Include(p => p.CatalogAttributes)
                .ThenInclude(ca => ca.Attribute)
                .Include(p => p.CatalogPictures)
                .Include(p => p.CatalogCategories)
                    .ThenInclude(cc => cc.Category)
                .SingleOrDefaultAsync(m => m.Id == id));

            if (ProductModel == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
