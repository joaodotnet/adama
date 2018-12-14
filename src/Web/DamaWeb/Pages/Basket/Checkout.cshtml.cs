using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.ViewModels;
using DamaWeb.Interfaces;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ApplicationCore.Entities.OrderAggregate;
using System.Text;
using DamaWeb.Extensions;
using ApplicationCore.Entities;
using ApplicationCore;
using Microsoft.Extensions.Options;
using System.Linq;
using ApplicationCore.DTOs;
using Microsoft.ApplicationInsights;

namespace DamaWeb.Pages.Basket
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
        private readonly TelemetryClient _telemetry;
        private readonly EmailSettings _settings;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            IUriComposer uriComposer,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOrderService orderService,
            IShopService shopService,
            IEmailSender emailSender,
            IOptions<EmailSettings> settings,
            TelemetryClient telemetry)
        {
            _basketService = basketService;
            _uriComposer = uriComposer;
            _userManager = userManager;
            _signInManager = signInManager;
            _orderService = orderService;
            _basketViewModelService = basketViewModelService;
            _shopService = shopService;
            _emailSender = emailSender;
            _telemetry = telemetry;
            _settings = settings.Value;
            
        }
        [BindProperty]
        public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

        [BindProperty]
        public AddressViewModel UserAddress { get; set; } = new AddressViewModel();

        [BindProperty]
        public bool WantInvoice { get; set; } = false;

        [BindProperty]
        public DeliveryTimeDTO DeliveryTime { get; set; } = new DeliveryTimeDTO();

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                UserAddress = await _shopService.GetUserAddress(user.Id);
                UserAddress.InvoiceTaxNumber = user.NIF;
                UserAddress.ContactPhoneNumber = await _userManager.GetPhoneNumberAsync(user);

            }
            await SetBasketModelAsync();

            //Set Min and Max Delivery time
            DeliveryTime = await _basketService.CalculateDeliveryTime(BasketModel.Id);
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid && ValidateAddressModel())
            {
                var user = await _userManager.GetUserAsync(User);
                //Save Address
                if (UserAddress.SaveAddress)
                {
                    await _userManager.SetPhoneNumberAsync(user, UserAddress.ContactPhoneNumber);
                    if(UserAddress.UseUserAddress == 1)
                        await _shopService.AddorUpdateUserAddress(user, UserAddress);
                }
                if (UserAddress.InvoiceSaveAddress)
                    await _shopService.AddorUpdateUserAddress(user, UserAddress, AddressType.BILLING);

                //Update Total if User
                decimal shippingcost = 0;
                Address address = new Address(UserAddress.Name, "Mercado de Loulé - Banca nº 44, Praça da Republica", "Loulé", "Portugal", "8100-270"); //,UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);
                if (UserAddress.UseUserAddress == 1)
                {
                    shippingcost = BasketModel.DefaultShippingCost;
                    address = new Address(UserAddress.Name, UserAddress.Street, UserAddress.City, UserAddress.Country, UserAddress.PostalCode); //, UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);                    
                }
                Address billingAddress = null;
                int? taxNumber = null;
                if (WantInvoice)
                {
                    taxNumber = UserAddress.InvoiceTaxNumber;
                    if (UserAddress.UseSameAsShipping)
                        billingAddress = new Address(UserAddress.InvoiceName, address.Street, address.City, address.Country, address.PostalCode);
                    else
                        billingAddress = new Address(UserAddress.InvoiceName, UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);
                }


                    //if(UserAddress.UseSameAsShipping && UserAddress.UseUserAddress.Value == 2)
                    //if (UserAddress.UseUserAddress == 1 && !UserAddress.UseSameAsShipping)
                    //    billingAddress = new Address(UserAddress.InvoiceName, UserAddress.ContactPhoneNumber, UserAddress.InvoiceAddressStreet, UserAddress.InvoiceAddressCity, UserAddress.InvoiceAddressCountry, UserAddress.InvoiceAddressPostalCode);
                    //else if (UserAddress.UseUserAddress == 1 && UserAddress.UseSameAsShipping)
                    //    billingAddress = new Address(UserAddress.InvoiceName, address.PhoneNumber, address.Street, address.City, address.Country, address.PostalCode);
                    //else if (UserAddress.UseUserAddress == 2)
                    //    billingAddress = new Address(UserAddress.InvoiceName, UserAddress.ContactPhoneNumber, UserAddress.InvoiceAddressStreet,UserAddress.InvoiceAddressCity,UserAddress.InvoiceAddressCountry,UserAddress.InvoiceAddressPostalCode);

                var resOrder = await _orderService.CreateOrderAsync(BasketModel.Id, UserAddress.ContactPhoneNumber, taxNumber, address, billingAddress, UserAddress.UseSameAsShipping, shippingcost);                
                if(resOrder != null)
                    _telemetry.TrackEvent("NewOrder");

                await _basketService.DeleteBasketAsync(BasketModel.Id);

                var body = GetEmailBody(resOrder, user, UserAddress.UseUserAddress == 2, DeliveryTime);
                await _emailSender.SendEmailAsync(_settings.FromOrderEmail, resOrder.BuyerId, $"Dama no Jornal® - Encomenda #{resOrder.Id}", body, _settings.CCEmails);

                return RedirectToPage("./Result", new { id = resOrder.Id } );
            }
            await SetBasketModelAsync();
            return Page();
        }

        private string GetEmailBody(ApplicationCore.Entities.OrderAggregate.Order order, ApplicationUser user, bool pickupAtStore, DeliveryTimeDTO deliveryTime)
        {
            var name = user != null && !string.IsNullOrEmpty(user.FirstName) ? $"{user.FirstName} {user.LastName}" : order.BuyerId;
            string body = $@"
<table style='width:550px;'>
        <tr>
            <td width='400px' style='vertical-align:bottom'>
                Olá <strong>{name}</strong><br />
                Obrigada por escolheres a Dama no Jornal®.<br />
                A tua encomenda foi criada com <strong>Sucesso!</strong> <br />
                O próximo passo será efectuares o pagamento com os dados que vais encontrar a baixo. <br />
                <strong>Obrigada!</strong><br />
            </td>
            <td>
                <img src='https://www.damanojornal.com/loja/images/dama_bird.png' width='150' />
            </td>
        </tr>
    </table>
    <div style='width:550px'>
        <img width='100%' src='https://www.damanojornal.com/loja/images/linha-coracao.png' />
    </div>
    <div>
        ENCOMENDA #{order.Id}<br />
        ESTADO: {EnumHelper<OrderStateType>.GetDisplayValue(order.OrderState)} <br />
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
                <td>Subtotal</td>
                <td style='text-align:right'>{order.Total() - order.ShippingCost} €</td>
            </tr>
            <tr>
                <td colspan='2'>
                    <hr />
                </td>
            </tr>
            <tr>
                <td>Envio</td>
                <td style='text-align:right'>{order.ShippingCost} €</td>
            </tr>
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
    body +=$@"<div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center'>INFORMAÇÕES DE ENVIO*</h3>
        <div style='text-align:center;width:550px'>
            <strong>{order.ShipToAddress.Name}</strong>";            
            if (order.TaxNumber.HasValue)
                body += $"({order.TaxNumber})";

            body += $@"<br />
            Telefone: {order.PhoneNumber}<br/>
            {order.ShipToAddress.Street}<br />
            {order.ShipToAddress.PostalCode} {order.ShipToAddress.City}
        </div>
        <div style='margin-top:20px;text-align:center;width:550px'>
            <strong>Tempo de entrega:</strong> {deliveryTime.Min} a {deliveryTime.Max} {EnumHelper<DeliveryTimeUnitType>.GetDisplayValue(deliveryTime.Unit)} úteis para artigos em stock
        </div>
    </div>
    <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center;width:550px'>MÉTODO DE ENVIO</h3>
        <div style='text-align:center;width:550px'>
            <strong>";
            if (pickupAtStore) body += "Recolha no nosso Ponto de Referência:"; else body += "Correio Registado em mão (CTT e CTT EXPRESSO)";
            body += "</strong><br />";
            if (pickupAtStore)
                body += @"
            Mercado Municipal de Loulé<br />
            <a href='https://goo.gl/maps/vHLacbNAqdo' style='color: #EF7C8D;text-decoration:underline'>Ver Mapa</a>";            
        body += $@"</div>

    </div>
    <div style='margin-top:20px;background-color:#eeebeb;width:550px;padding: 5px;'>
        <h3 style='text-align:center;width:550px'>INFORMAÇÕES DE PAGAMENTO</h3>
        <div style='text-align:center;width:550px'>
            <h4>Para concluir a sua encomenda por favor faça uma transferência bancária com os seguintes dados:</h4>
        </div>
        <div style='margin-top:20px;text-align:center;width:550px'>
            Valor: {order.Total()} €<br />
            IBAN PT50004572114025360687172<br />
            NIB 004572114025360687172<br />
            CAIXA DE CRÉDITO AGRÍCOLA<br />
            <strong>Titular da conta:</strong> Susana Nunes<br />
        </div>
        <div style='margin-top:20px;text-align:center;width:550px'>
            <span>E envie o comprovativo de pagamento em resposta a este email, ou envie um mail para encomendas@damanojornal.com indicando a referência de encomenda nº {order.Id}.</span>
        </div>
    </div>
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Se tem alguma dúvida sobre a sua encomenda, por favor contacte-nos.</strong>
    </div>
    <div style='margin-top:20px;text-align:center;width:550px'>
        <strong>Muito Obrigada,</strong>
    </div>
    <div style='color: #EF7C8D;text-align:center;width:550px'>
        ❤ Dama no Jornal®
    </div>
    <div style='text-align:center;width:550px'>
        <img width='100' src='https://www.damanojornal.com/loja/images/logo_name.png' />
    </div>";
            return body;
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
                if (string.IsNullOrEmpty(UserAddress.Name))
                    ModelState.AddModelError("UserAddress.Name", "O campo Nome é obrigatório");
                if (string.IsNullOrEmpty(UserAddress.ContactPhoneNumber))
                    ModelState.AddModelError("UserAddress.ContactPhoneNumber", "O campo Telefone é obrigatório");
                if (string.IsNullOrEmpty(UserAddress.Street))
                    ModelState.AddModelError("UserAddress.Street", "O campo Morada é obrigatório.");
                if (string.IsNullOrEmpty(UserAddress.City))
                    ModelState.AddModelError("UserAddress.City", "O campo Cidade é obrigatório.");
                if (string.IsNullOrEmpty(UserAddress.PostalCode))
                    ModelState.AddModelError("UserAddress.PostalCode", "O campo Código Postal é obrigatório.");
                if (string.IsNullOrEmpty(UserAddress.Country))
                    ModelState.AddModelError("UserAddress.Country", "O campo País é obrigatório.");                
            }
            else if(UserAddress.UseUserAddress == 2)
            {
                UserAddress.Street = "Mercado de Loulé - Banca nº 44, Praça da Republica";
                UserAddress.City = "Loulé";
                UserAddress.PostalCode = "8100-270";
                UserAddress.Country = "Portugal";
            }

            if (WantInvoice)
            {
                if (!UserAddress.InvoiceTaxNumber.HasValue)
                    ModelState.AddModelError("UserAddress.InvoiceTaxNumber", "O campo NIF é obrigatório");

                if (!UserAddress.UseSameAsShipping)
                {
                    if (string.IsNullOrEmpty(UserAddress.InvoiceName))
                        ModelState.AddModelError("UserAddress.InvoiceName", "O campo Nome (Faturação) é obrigatório");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressStreet))
                        ModelState.AddModelError("UserAddress.InvoiceAddressStreet", "O campo Morada (Faturação) é obrigatório.");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressCity))
                        ModelState.AddModelError("UserAddress.InvoiceAddressCity", "O campo Cidade (Faturação) é obrigatório.");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressPostalCode))
                        ModelState.AddModelError("UserAddress.InvoiceAddressPostalCode", "O campo Código Postal (Faturação) é obrigatório.");
                    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressCountry))
                        ModelState.AddModelError("UserAddress.InvoiceAddressCountry", "O campo País (Faturação) é obrigatório.");
                }
            }

            //if(UserAddress.UseUserAddress == 1 && !UserAddress.UseSameAsShipping)
            //{
            //    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressStreet))
            //        ModelState.AddModelError("UserAddress.InvoiceAddressStreet", "O campo Morada (Faturação) é obrigatório.");
            //    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressCity))
            //        ModelState.AddModelError("UserAddress.InvoiceAddressCity", "O campo Cidade (Faturação) é obrigatório.");
            //    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressPostalCode))
            //        ModelState.AddModelError("UserAddress.InvoiceAddressPostalCode", "O campo Código Postal (Faturação) é obrigatório.");
            //    if (string.IsNullOrEmpty(UserAddress.InvoiceAddressCountry))
            //        ModelState.AddModelError("UserAddress.InvoiceAddressCountry", "O campo País (Faturação) é obrigatório.");
            //}
            
            return ModelState.IsValid;
        }

    }    
}
