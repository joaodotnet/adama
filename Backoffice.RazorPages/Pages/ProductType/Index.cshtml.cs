using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.RazorPages.ViewModels;
using AutoMapper;

namespace Backoffice.RazorPages.Pages.ProductType
{
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public IndexModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IList<ProductTypeViewModel> ProductTypesModel { get;set; }

        public async Task OnGetAsync()
        {
            var types = await _context.ProductTypes
                .Include(p => p.Category).ToListAsync();

            ProductTypesModel = _mapper.Map<List<ProductTypeViewModel>>(types);

        }
    }
}
