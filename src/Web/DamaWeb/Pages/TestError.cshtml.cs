using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DamaWeb.Pages
{
    public class TestErrorModel : PageModel
    {
        public void OnGet()
        {
            throw new Exception("teste com envio de mail");
        }
    }
}