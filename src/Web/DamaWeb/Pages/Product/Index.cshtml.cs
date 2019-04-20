using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DamaWeb.Pages.Product
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet(string id)
        {
            return RedirectPermanent(Url.Page("/Product", new { id }));
        }
    }
}
