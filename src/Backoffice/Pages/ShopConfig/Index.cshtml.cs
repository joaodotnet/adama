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
    public class IndexModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        private readonly IMapper _mapper;

        public IndexModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public IList<ShopConfigViewModel> ShopConfigModel { get;set; }        

        public async Task OnGetAsync()
        {
            var list = await _context.ShopConfigs
                .Include(x => x.Details)
                .ToListAsync();

            //var details = await _context.ShopConfigDetails
            //    .Include(s => s.ShopConfig).ToListAsync();

            ShopConfigModel = _mapper.Map<List<ShopConfigViewModel>>(list);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var item in ShopConfigModel)
            {
                var shopConfig = _mapper.Map<ApplicationCore.Entities.ShopConfig>(item);
                _context.Attach(shopConfig).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
            return RedirectToPage("./Index");
        }
    }
}
