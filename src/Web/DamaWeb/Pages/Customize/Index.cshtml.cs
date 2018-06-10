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

namespace DamaWeb.Pages.Customize
{
    public class IndexModel : PageModel
    {
        private readonly ICustomizeViewModelService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ICustomizeViewModelService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [BindProperty]
        public CustomizeViewModel CustomizeModel { get; set; }

        [FromQuery]
        public int? CategoryId { get; set; }

        [FromQuery]
        public int? CatalogItemId { get; set; }

        public async Task OnGetAsync()
        {            
            CustomizeModel = await _service.GetCustomizeItems(CategoryId, CatalogItemId);
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                CustomizeModel.BuyerEmail = user.Email;
                CustomizeModel.BuyerName = $"{user.FirstName} {user.LastName}";
                CustomizeModel.BuyerPhone = user.PhoneNumber;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _service.SendCustomizeService(CustomizeModel);
            return RedirectToPage("./Result");
        }
    }
}