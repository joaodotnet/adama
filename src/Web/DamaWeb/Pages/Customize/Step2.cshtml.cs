using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using Newtonsoft.Json;

namespace DamaWeb.Pages.Customize
{
    public class Step2Model : PageModel
    {
        private readonly ICustomizeViewModelService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public Step2Model(ICustomizeViewModelService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [BindProperty]
        public CustomizeViewModel CustomizeModel { get; set; } = new CustomizeViewModel();

        [FromQuery]
        public int? CategoryId { get; set; }

        [FromQuery]
        public int? CatalogItemId { get; set; }

        public void OnGet(int id)
        {
            CustomizeModel.CatalogItemId = id;
        }
    }
}