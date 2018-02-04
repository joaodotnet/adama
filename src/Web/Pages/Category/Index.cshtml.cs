using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamaShopWeb.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;

namespace Web.Pages.Category
{
    public class IndexModel : PageModel
    {
        private readonly IShopService _shopService;
        public IndexModel(IShopService service)
        {
            _shopService = service;
        }
        [TempData]
        public string CategoryName { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            var cat = await _shopService.GetCategory(id);
            if (cat == null)
                return NotFound();
            else
                CategoryName = cat.Name;

            return Page();
        }
    }
}