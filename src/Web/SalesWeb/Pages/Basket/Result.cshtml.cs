using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SalesWeb.Pages.Basket
{
    public class ResultModel : PageModel
    {
        public int OrderNumber { get; set; }
        public void OnGet(int id)
        {
            OrderNumber = id;
        }
    }
}