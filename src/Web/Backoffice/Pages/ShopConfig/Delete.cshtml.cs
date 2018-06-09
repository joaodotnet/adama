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

namespace Backoffice.Pages.ShopConfig
{
    public class DeleteModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public DeleteModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public ShopConfigDetailViewModel ShopConfigDetailModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopConfigDetail = await _context.ShopConfigDetails
                .Include(s => s.ShopConfig).SingleOrDefaultAsync(m => m.Id == id);

            if (shopConfigDetail == null)
            {
                return NotFound();
            }

            ShopConfigDetailModel = _mapper.Map<ShopConfigDetailViewModel>(shopConfigDetail);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detail = await _context.ShopConfigDetails.FindAsync(id);

            if (detail != null)
            {
                _context.ShopConfigDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
