using ApplicationCore.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;

namespace SalesWeb.Pages.Account.Manage
{
    public class AddressesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;        
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IShopService _shopService;

        public AddressesModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ChangePasswordModel> logger,
            IShopService shopService)
        {
            _userManager = userManager;
            _logger = logger;
            _shopService = shopService;
        }

        [BindProperty]
        public AddressViewModel UserAddress { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UserAddress = await _shopService.GetUserAddress(user.Id);            

            return Page();
        }
    }
}
