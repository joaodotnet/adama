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

namespace Backoffice.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public IndexModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IList<ProductViewModel> ProductModel { get;set; }

        public async Task OnGetAsync()
        {
            ProductModel = _mapper.Map<List<ProductViewModel>>(await _context.CatalogItems
                .Include(p => p.CatalogIllustration)
                    .ThenInclude(i => i.IllustrationType)
                .Include(p => p.CatalogType)
                .Include(p => p.CatalogAttributes)
                .ToListAsync());

            foreach (var item in ProductModel)
            {
                if(item.ProductAttributes.Count > 0)
                {
                    var skus = string.Join("<br>", item.ProductAttributes.Select(x => x.ProductSKU));
                    item.ProductSKU = skus;
                }
            }
        }
    }
}
