using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.ViewModels;
using Web.Interfaces;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderAggregate;

namespace Web.Pages.Basket
{
    public class CheckoutModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly IUriComposer _uriComposer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrderService _orderService;
        private string _username = null;
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IShopService _shopService;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            IUriComposer uriComposer,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService,
            IShopService shopService)
        {
            _basketService = basketService;
            _uriComposer = uriComposer;
            _userManager = userManager;
            _signInManager = signInManager;
            _orderService = orderService;
            _basketViewModelService = basketViewModelService;
            _shopService = shopService;
        }
        [BindProperty]
        public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

        [BindProperty]
        public AddressViewModel UserAddress { get; set; } = new AddressViewModel();

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                UserAddress = await _shopService.GetUserAddress(user.Id);
            }
            await SetBasketModelAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid && ValidateAddressModel())
            {
                //Save Address
                if (UserAddress.SaveAddress)
                {
                    var user = await _userManager.GetUserAsync(User);
                    await _shopService.AddorUpdateUserAddress(user, UserAddress);
                }

                //Update Total if User
                decimal shippingcost = 0;
                Address address = new Address();
                if (UserAddress.UseUserAddress == 1)
                {
                    shippingcost = BasketModel.DefaultShippingCost;
                    address = new Address(UserAddress.Street, UserAddress.City, UserAddress.Country, UserAddress.PostalCode);
                }

                await _orderService.CreateOrderAsync(BasketModel.Id, address, shippingcost);

                await _basketService.DeleteBasketAsync(BasketModel.Id);

                return RedirectToPage("./Result");
            }
            return Page();
        }

        //public async Task<IActionResult> OnPostCreateOrder(Dictionary<string, int> items)
        //{
        //    //await SetBasketModelAsync();

        //    //await _basketService.SetQuantities(BasketModel.Id, items);


        //    await _orderService.CreateOrderAsync(BasketModel.Id, new Address("123 Main St.", "Kent", "OH", "United States", "44240"));

        //    await _basketService.DeleteBasketAsync(BasketModel.Id);

        //    return RedirectToPage();
        //}

        private async Task SetBasketModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetBasketCookieAndUserName();
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }
        }

        private void GetOrSetBasketCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {
                _username = Request.Cookies[Constants.BASKET_COOKIENAME];
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
        }

        private bool ValidateAddressModel()
        {
            if (UserAddress.UseUserAddress == 1)
            {
                if (string.IsNullOrEmpty(UserAddress.Street))
                    ModelState.AddModelError("UserAddress.Street", "O campo Morada é obrigatório.");
                if (string.IsNullOrEmpty(UserAddress.City))
                    ModelState.AddModelError("UserAddress.City", "O campo Cidade é obrigatório.");
                if (string.IsNullOrEmpty(UserAddress.PostalCode))
                    ModelState.AddModelError("UserAddress.PostalCode", "O campo Código Postal é obrigatório.");
                if (string.IsNullOrEmpty(UserAddress.Country))
                    ModelState.AddModelError("UserAddress.Country", "O campo País é obrigatório.");
                return ModelState.IsValid;
            }
            return true;
        }

    }
}
