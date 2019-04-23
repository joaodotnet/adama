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
                .Include(p => p.CatalogCategories)
                    .ThenInclude( cc => cc.Category)
                    .OrderByDescending(p => p.Id)
                .ToListAsync());

            foreach (var item in ProductModel)
            {
                if(!item.Price.HasValue || item.Price == 0)
                {
                    item.Price = item.CatalogType.Price;
                }
            }
        }

        public async Task<IActionResult> OnGetUpdateProductAsync(int id, int checkboxType, bool value)
        {
            var product = await _context.CatalogItems.FindAsync(id);
            if (checkboxType == 1)
                product.ShowOnShop = value;
            else if (checkboxType == 2)
                product.IsNew = value;
            else if (checkboxType == 3)
                product.IsFeatured = value;
            else if (checkboxType == 4)
                product.CanCustomize = value;

            await _context.SaveChangesAsync();
            return new JsonResult("OK");
        }
    }
}
