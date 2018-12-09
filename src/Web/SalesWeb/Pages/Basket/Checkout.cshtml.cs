using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesWeb.ViewModels;
using SalesWeb.Interfaces;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderAggregate;
using System.Text;
using SalesWeb.Extensions;
using ApplicationCore.Entities;
using ApplicationCore;
using Microsoft.Extensions.Options;
using System.Linq;
using ApplicationCore.DTOs;
using Microsoft.ApplicationInsights;
using ApplicationCore.Exceptions;

namespace SalesWeb.Pages.Basket
{
    public class CheckoutModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IEmailSender _emailSender;
        private readonly IAuthConfigRepository _authConfigRepository;
        private readonly AppSettings _settings;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,            
            UserManager<ApplicationUser> userManager,
            IOrderService orderService,            
            IEmailSender emailSender,
            IOptions<AppSettings> settings,
            IAuthConfigRepository authConfigRepository)
        {
            _basketService = basketService;
            _userManager = userManager;
            _orderService = orderService;
            _basketViewModelService = basketViewModelService;            
            _emailSender = emailSender;
            _authConfigRepository = authConfigRepository;
            _settings = settings.Value;
            
        }
        [BindProperty]
        public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

        [BindProperty]
        public AddressViewModel UserAddress { get; set; } = new AddressViewModel();

        [BindProperty]
        public bool WantInvoice { get; set; } = false;

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            //Check if has auth tokens
            var authTokens = await _authConfigRepository.GetAuthConfigAsync(ApplicationCore.Entities.DamaApplicationId.SALESWEB);
            if (authTokens == null)
                return Redirect(string.Format(_settings.Sage.AuthorizationURL, _settings.Sage.ClientId, _settings.Sage.CallbackURL));

            await SetBasketModelAsync();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid && ValidateAddressModel())
            {
                var user = await _userManager.GetUserAsync(User);

                Address address = new Address(UserAddress.Name, "Mercado de Loulé - Banca nº 44, Praça da Republica", "Loulé", "Portugal", "8100-270"); //,UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);
              
                Address billingAddress = null;
                int? taxNumber = null;
                if (WantInvoice)
                {
                    taxNumber = UserAddress.InvoiceTaxNumber;
                    billingAddress = new Address(UserAddress.InvoiceName, UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);
                }


                    //if(UserAddress.UseSameAsShipping && UserAddress.UseUserAddress.Value == 2)
                    //if (UserAddress.UseUserAddress == 1 && !UserAddress.UseSameAsShipping)
                    //    billingAddress = new Address(UserAddress.InvoiceName, UserAddress.ContactPhoneNumber, UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);
                    //else if (UserAddress.UseUserAddress == 1 && UserAddress.UseSameAsShipping)
                    //    billingAddress = new Address(UserAddress.InvoiceName, address.PhoneNumber, address.Street, address.City, address.Country, address.PostalCode);
                    //else if (UserAddress.UseUserAddress == 2)
                    //    billingAddress = new Address(UserAddress.InvoiceName, UserAddress.ContactPhoneNumber, UserAddress.InvoiceAddressStreet,UserAddress.InvoiceAddressCity,UserAddress.InvoiceAddressCountry,UserAddress.InvoiceAddressPostalCode);

                var resOrder = await _orderService.CreateOrderAsync(BasketModel.Id, UserAddress.ContactPhoneNumber, taxNumber, address, billingAddress, false, 0, UserAddress.InvoiceEmail);                

                await _basketService.DeleteBasketAsync(BasketModel.Id);

                if (WantInvoice)
                {
                    var body = GetEmailBody(resOrder, user);
                    try
                    {
                        await _emailSender.SendEmailAsync(_settings.Email.FromOrderEmail,
                                        resOrder.CustomerEmail,
                                        $"SaborComTradicao ® - Encomenda #{resOrder.Id}",
                                        body,
                                        _settings.Email.CCEmails);
                    }
                    catch (SendEmailException ex)
                    {
                        StatusMessage = $"Erro ao enviar email: {ex}";
                        return RedirectToPage("/Index");
                    }
                }
                StatusMessage = "O registo de venda foi efectuado com sucesso";
                return RedirectToPage("/Index");
            }
            await SetBasketModelAsync();
            return Page();
        }

        private string GetEmailBody(ApplicationCore.Entities.OrderAggregate.Order order, ApplicationUser user)
        {
            string body = $@"
<table style='width:550px;'>
        <tr>
            <td width='400px' style='vertical-align:bottom'>
                Olá <strong>{user.FirstName} {user.LastName}</strong><br />
                Obrigada por escolher a Sabor com Tradição®.<br />
                A sua fatura segue em anexo.<br />
            </td>
            <td>
                <img src='https://www.damanojornal.com/loja/images/dama_bird.png' width='150' />
            </td>
        </tr>
    </table>
    <div style='width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>
   
    <div style='margin-top:20px;width:550px'>
        <table width='100%'>";

            foreach (var item in order.OrderItems)
            {
                //Get Attribtues
                var attributes = _orderService.GetOrderAttributes(item.ItemOrdered.CatalogItemId, item.CatalogAttribute1, item.CatalogAttribute2, item.CatalogAttribute3);
                body += $@"
            <tr>
                <td width='250px'>
                    <img width='250' src='{item.ItemOrdered.PictureUri}' />
                </td>
                <td style='padding-bottom:20px;vertical-align:bottom'>
                    <table>
                        <tr>
                            <td>{item.ItemOrdered.ProductName}</td>
                            <td>{item.UnitPrice} €</td>
                        </tr>";
                if(!string.IsNullOrEmpty(item.CustomizeName))
                {
                    var sideText = item.CustomizeSide;
                    if (item.CustomizeSide.LastIndexOf('-') > 0)
                        sideText = item.CustomizeSide.Substring(item.CustomizeSide.LastIndexOf('-') + 1);

                    body += $@"<tr>
                            <td>Personalização: {item.CustomizeName} ({sideText})</td>
                        </tr>";
                }
                foreach (var attr in attributes)
                {
                    body += $@"<tr>
                            <td>{EnumHelper<AttributeType>.GetDisplayValue(attr.Type)}: {attr.Name}</td>
                        </tr>";
                }
                body +=$@"<tr>
                            <td>Quantidade: {item.Units}</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan='2'><hr /></td>
            </tr>";
            }

            body += $@"</table>
    </div>
    <div style='margin-top:20px;width:550px'>
        <table width='100%'>           
            <tr>
                <td colspan='2'>
                    <hr />
                </td>
            </tr>
            <tr>
                <td><strong>Total</strong></td>
                <td style='text-align:right'>{order.Total()} €</td>
            </tr>
            <tr>
                <td colspan='2'>
                    <hr />
                </td>
            </tr>
        </table>
    </div>
    <div style='margin-top:20px;width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>";
            if(WantInvoice)
            { 
    body +=$@"<div style='background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='margin-top:20px;text-align:center;width:550px'>INFORMAÇÔES DE FACTURAÇÂO</h3>
        <div style='text-align:center;width:550px'>
            <strong>{order.BillingToAddress.Name}</strong>";
            if (order.TaxNumber.HasValue)
                body += $"({order.TaxNumber})";
                body += $@"
            <br />
            {order.BillingToAddress.Street}<br />
            {order.BillingToAddress.PostalCode} {order.BillingToAddress.City}
        </div>
    </div>";
            }
    body += @"
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Se tem alguma dúvida sobre a sua encomenda, por favor contacte-nos.</strong>
    </div>
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Muito Obrigada,</strong>
    </div>
    <div style='color: #EF7C8D;text-align:center;width:550px'>
        ❤ Sabor com Tradição®
    </div>
    <div style='text-align:center;width:550px'>
        <img width='100' src='https://www.damanojornal.com/loja/images/logo_name.png' />
    </div>";
            return body;
        }        

        private async Task SetBasketModelAsync()
        {
            //if (_signInManager.IsSignedIn(HttpContext.User))
            //{
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            //}
            //else
            //{
            //    GetOrSetBasketCookieAndUserName();
            //    BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            //}
        }

        //private void GetOrSetBasketCookieAndUserName()
        //{
        //    if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
        //    {
        //        _username = Request.Cookies[Constants.BASKET_COOKIENAME];
        //    }
        //    if (_username != null) return;

        //    _username = Guid.NewGuid().ToString();
        //    var cookieOptions = new CookieOptions();
        //    cookieOptions.Expires = DateTime.Today.AddYears(10);
        //    Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
        //}

        private bool ValidateAddressModel()
        {
            if (WantInvoice)
            {
                if (string.IsNullOrEmpty(UserAddress.InvoiceEmail))
                    ModelState.AddModelError("UserAddress.InvoiceEmail", "O campo Email do cliente é obrigatório");
                if (UserAddress.InvoiceTaxNumber.HasValue)
                {
                    if (string.IsNullOrEmpty(UserAddress.InvoiceName))
                        ModelState.AddModelError("UserAddress.InvoiceName", "O campo Nome é obrigatório");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressStreet))
                        ModelState.AddModelError("UserAddress.InvoiceAddressStreet", "O campo Morada é obrigatório.");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressCity))
                        ModelState.AddModelError("UserAddress.InvoiceAddressCity", "O campo Cidade é obrigatório.");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressPostalCode))
                        ModelState.AddModelError("UserAddress.InvoiceAddressPostalCode", "O campo Código Postal é obrigatório.");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressCountry))
                        ModelState.AddModelError("UserAddress.InvoiceAddressCountry", "O campo País é obrigatório.");
                }
            }
            return ModelState.IsValid;
        }

    }    
}
