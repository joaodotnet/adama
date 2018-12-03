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
using Microsoft.AspNetCore.Http;

namespace DamaWeb.Pages.Customize
{
    public class Step2Model : PageModel
    {
        private readonly ICustomizeViewModelService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IBasketViewModelService _basketViewModelService;
        private string _username = null;

        public Step2Model(ICustomizeViewModelService service, 
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IBasketViewModelService basketViewModelService)
        {
            _service = service;
            _userManager = userManager;
            _signInManager = signInManager;
            _basketViewModelService = basketViewModelService;
        }

        [BindProperty]
        public CustomizeViewModel CustomizeModel { get; set; } = new CustomizeViewModel();

        [FromQuery]
        public int? CategoryId { get; set; }

        [FromQuery]
        public int? CatalogItemId { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet(int id)
        {
            CustomizeModel.CatalogItemId = id;
        }

        public async Task<IActionResult> OnPostAsync(CustomizeViewModel model)
        {
            CustomizeModel = model;

            var basketId = await SetBasketModelAsync();
       
            await _service.AddCustomizeItemToBasket(basketId, model);

            //var user = await _userManager.GetUserAsync(User);
            //if (user != null)
            //{
            //    CustomizeModel.BuyerEmail = user.Email;
            //    CustomizeModel.BuyerName = $"{user.FirstName} {user.LastName}";
            //    CustomizeModel.BuyerPhone = user.PhoneNumber;
            //}
            StatusMessage = "A sua personalização foi adicionado ao carrinho!";
            return RedirectToPage("./Index");
        }

        private async Task<int> SetBasketModelAsync()
        {
            BasketViewModel model;
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                model = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetBasketCookieAndUserName();
                model = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }
            return model.Id;
        }

        private void GetOrSetBasketCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                _username = Request.Cookies[Constants.BASKET_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Today.AddYears(1),
                IsEssential = true
            };
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);

        }
    }
}