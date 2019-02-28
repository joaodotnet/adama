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
            await Initialize();

            //var details = await _context.ShopConfigDetails
            //    .Include(s => s.ShopConfig).ToListAsync();

            
        }

        private async Task Initialize()
        {
            List<ApplicationCore.Entities.ShopConfig> list = await _context.ShopConfigs
                            .Include(x => x.Details)
                            .OrderByDescending(x => x.Type)
                            .ToListAsync();
                            

            ShopConfigModel = _mapper.Map<List<ShopConfigViewModel>>(list);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await Initialize();
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

        public async Task<IActionResult> OnPostUpdateSEOAsync()
        {
            var input = ShopConfigModel.SingleOrDefault(x => x.Type == ShopConfigType.SEO);
            if (input != null)
            {
                if (string.IsNullOrEmpty(input.Value) || input.Value.Length > 160)
                {
                    await Initialize();
                    ModelState.AddModelError("", "Meta Description é inválida, tem que ter menos de 160 carateres!");
                    return Page();
                }
                else
                {
                    var shopConfig = _mapper.Map<ApplicationCore.Entities.ShopConfig>(input);
                    _context.Attach(shopConfig).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage("./Index");
        }
    }
}
