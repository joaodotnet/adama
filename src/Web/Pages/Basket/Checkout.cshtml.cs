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
using System.Text;
using Web.Extensions;
using ApplicationCore.Entities;

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
        private readonly IEmailSender _emailSender;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            IUriComposer uriComposer,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService,
            IShopService shopService,
            IEmailSender emailSender)
        {
            _basketService = basketService;
            _uriComposer = uriComposer;
            _userManager = userManager;
            _signInManager = signInManager;
            _orderService = orderService;
            _basketViewModelService = basketViewModelService;
            _shopService = shopService;
            _emailSender = emailSender;
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
                var user = await _userManager.GetUserAsync(User);
                //Save Address
                if (UserAddress.SaveAddress)
                {
                    await _shopService.AddorUpdateUserAddress(user, UserAddress);
                }

                //Update Total if User
                decimal shippingcost = 0;
                Address address = new Address("Mercado de Loulé - Banca nº 44, Praça da Republica","Loulé","Portugal", "8100-270");
                if (UserAddress.UseUserAddress == 1)
                {
                    shippingcost = BasketModel.DefaultShippingCost;
                    address = new Address(UserAddress.Street, UserAddress.City, UserAddress.Country, UserAddress.PostalCode);
                }

                var resOrder = await _orderService.CreateOrderAsync(BasketModel.Id, address, shippingcost);                

                await _basketService.DeleteBasketAsync(BasketModel.Id);

                var body = GetEmailBody(resOrder, user);
                await _emailSender.SendEmailAsync(resOrder.BuyerId, $"Encomenda nº  {resOrder.Id} foi criada com sucesso.", body);

                return RedirectToPage("./Result");
            }
            return Page();
        }

        private string GetEmailBody(ApplicationCore.Entities.OrderAggregate.Order order, ApplicationUser user)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine($"Olá {user.UserName},<br>");
            body.AppendLine("<br>");
            body.AppendLine("<br>");
            body.AppendLine("A sua encomenda foi criada com <strong>sucesso.</strong><br>");
            body.AppendLine("<br>");
            body.AppendLine("<br>");
            body.AppendLine($"<strong>Encomenda nº {order.Id}</strong><br>");
            body.AppendLine("<br>");
            foreach (var item in order.OrderItems)
            {
                body.AppendLine($"<img src='{item.ItemOrdered.PictureUri}' width='100px'/><br>");
                body.AppendLine($"{ item.Units}x {item.ItemOrdered.ProductName} € {item.UnitPrice}<br>");
                foreach (var attr in item.Details)
                {
                    body.AppendLine($"{EnumHelper<CatalogAttributeType>.GetDisplayValue(attr.AttributeType)} {attr.AttributeName}<br>");
                }
            }
            if(order.ShippingCost > 0)
                body.AppendLine($"<strong>Portes</strong> € {order.ShippingCost}<br>");
            body.AppendLine($"<strong>Total</strong> € {order.Total()}<br>");

            body.AppendLine("<br>");
            body.AppendLine("<br>");
            body.AppendLine($"<strong>Morada de Entrega</strong><br>");
            if (string.IsNullOrEmpty(order.ShipToAddress.Street))
            {
                body.AppendLine($"Mercado de Loulé, Praça da Republica, 8100-270 Loulé<br>");
                body.AppendLine($"Banca Nº 44<br>");
                body.AppendLine("<br>");
                body.AppendLine("<strong><a href='https://goo.gl/maps/3BbPSkX5N7H2'>Mapa</a></strong>");
            }
            else
            {
                body.AppendLine($"{order.ShipToAddress.Street}, {order.ShipToAddress.PostalCode}, {order.ShipToAddress.City}, {order.ShipToAddress.Country}<br>");
            }
            body.AppendLine("<br>");
            body.AppendLine("<br>");
            body.AppendLine($"<strong>Para concluir a sua encomenda por favor faça a transferência no valor de {order.Total()} para o IBAN  XXXXXXXXXXXXXXXXXXXXXXXXX.</strong> e envie o comprovativo respondendo a este email, ou enviando para info@damanojornal.com indicando a referência nº {order.Id}.<br>");
            body.AppendLine("<br>");
            body.AppendLine("<br>");
            body.AppendLine("Alguma dúvida não hesite em contactar-nos.<br>");
            body.AppendLine("Dama no Jornal<br>");
            body.AppendLine($"<img src='{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{HttpContext.Request.PathBase}/{Url.Content("/images/damanojornal-email.jpg")}' >");
            return body.ToString();
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
