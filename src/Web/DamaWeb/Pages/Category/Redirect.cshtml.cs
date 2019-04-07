using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DamaWeb.Pages.Category
{
    public class RedirectModel : PageModel
    {
        public IActionResult OnGet(string id, int? p)
        {
            return RedirectPermanent(Url.Page("/Category/Index", new { id, p }));
        }
    }
}
