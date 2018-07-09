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
            //CustomizeModel = await _service.GetCustomizeItems(CategoryId, CatalogItemId);
            //var user = await _userManager.GetUserAsync(User);
            //if (user != null)
            //{
            //    CustomizeModel.BuyerEmail = user.Email;
            //    CustomizeModel.BuyerName = $"{user.FirstName} {user.LastName}";
            //    CustomizeModel.BuyerPhone = user.PhoneNumber;
            //}
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("./Step3");
            //return RedirectToPage("./Step3", new { id = CustomizeModel.ProductSelected, colors = CustomizeModel.Colors, description = CustomizeModel.Description, text = CustomizeModel.Text });
            //await _service.SendCustomizeService(CustomizeModel);
            //return RedirectToPage("./Result");
        }
    }
}