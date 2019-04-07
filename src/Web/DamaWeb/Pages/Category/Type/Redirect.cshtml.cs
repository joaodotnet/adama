using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DamaWeb.Pages.Category.Type
{
    public class RedirectModel : PageModel
    {
        public IActionResult OnGet(string cat, string type, int? p)
        {
            return RedirectPermanent(Url.Page("/Category/Type/Index", new { cat, type, p }));
        }
    }
}
