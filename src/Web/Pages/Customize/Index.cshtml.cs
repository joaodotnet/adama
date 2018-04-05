using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Customize
{
    public class IndexModel : PageModel
    {
        private readonly ICustomizeViewModelService _service;

        public IndexModel(ICustomizeViewModelService service)
        {
            _service = service;
        }

        [BindProperty]
        public CustomizeViewModel CustomizeModel { get; set; }

        [FromQuery]
        public int? CategoryId { get; set; }

        [FromQuery]
        public int? CatalogId { get; set; }

        public async Task OnGetAsync()
        {
            CustomizeModel = await _service.GetCustomizeItems(CategoryId);
        }
    }
}