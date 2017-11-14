using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.ProductType
{
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public DetailsModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ProductTypeViewModel ProductTypeModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var type = await _context.ProductTypes
                .Include(p => p.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (type == null)
            {
                return NotFound();
            }
            ProductTypeModel = _mapper.Map<ProductTypeViewModel>(type);
            return Page();
        }
    }
}
