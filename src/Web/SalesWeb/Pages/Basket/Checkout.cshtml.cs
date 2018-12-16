using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using SalesWeb.Interfaces;
using SalesWeb.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        private readonly IInvoiceService _invoiceService;
        private readonly IAsyncRepository<Country> _countryRepository;
        private readonly AppSettings _settings;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            UserManager<ApplicationUser> userManager,
            IOrderService orderService,
            IEmailSender emailSender,
            IOptions<AppSettings> settings,
            IAuthConfigRepository authConfigRepository,
            IInvoiceService invoiceService,
            IAsyncRepository<Country> countryRepository)
        {
            _basketService = basketService;
            _userManager = userManager;
            _orderService = orderService;
            _basketViewModelService = basketViewModelService;
            _emailSender = emailSender;
            _authConfigRepository = authConfigRepository;
            _invoiceService = invoiceService;
            _countryRepository = countryRepository;
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
            var authTokens = await _authConfigRepository.GetAuthConfigAsync(SageApplicationType.SALESWEB);
            if (string.IsNullOrEmpty(authTokens.AccessToken))
                return Redirect(string.Format(_settings.Sage.AuthorizationURL, authTokens.ClientId, authTokens.CallbackURL));

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

                ApplicationCore.Entities.OrderAggregate.Order resOrder;
                try
                {
                    resOrder = await _orderService.CreateOrderAsync(BasketModel.Id, UserAddress.ContactPhoneNumber, taxNumber, address, billingAddress, false, 0, UserAddress.InvoiceEmail, true, PaymentType.CASH);
                }
                catch (RegisterInvoiceException ex)
                {
                    //TODO Remover encomenda
                    StatusMessage = $"Erro {ex}";
                    return RedirectToPage("/Index");
                }

                await _basketService.DeleteBasketAsync(BasketModel.Id);

                if (WantInvoice)
                {
                    //var body = GetEmailBody(resOrder, user);
                    var invoiceBytes = await _invoiceService.GetPDFInvoiceAsync(SageApplicationType.SALESWEB, resOrder.SalesInvoiceId.Value);
                    if (invoiceBytes != null)
                    {
                        //Send Email to client (from: info.saborcomtradicao@gmail.com)

                        var body = $"<strong>Olá!</strong><br>" +
                            $"Obrigada por comprares na Sabor com Tradição.<br>" +
                            $"Enviamos em anexo a fatura relativa à tua encomenda. <br>" +
                            "<br>Muito Obrigada.<br>" +
                            "<br>--------------------------------------------------<br>" +
                            "<br><strong>Hi!</strong><br>" +
                            "Thank you to shopping at Sabor Com Tradição in Loulé, Portugal. <br>" +
                            "We send as attach the invoice relates to your order.<br>" +
                            "<br>Thank you.<br>" +
                            "<br>Sabor com Tradição" +
                            "<br>http://www.saborcomtradicao.com";

                        List<(string, byte[])> files = new List<(string, byte[])>();
                        files.Add(($"FaturaSaborComTradicao#{resOrder.Id}.pdf", invoiceBytes));

                        try
                        {
                            await _emailSender.SendEmailAsync(
                            _settings.Email.FromOrderEmail,
                            resOrder.CustomerEmail,
                            $"Sabor com Tradição - Encomenda #{resOrder.Id}",
                            body,
                            _settings.Email.CCEmails,
                            null,
                            files);
                        }
                        catch (SendEmailException ex)
                        {
                            StatusMessage = $"Erro ao enviar email: {ex}";
                            return RedirectToPage("/Index");
                        }
                    }
                    else
                    {
                        StatusMessage = "Erro: O registo de venda foi efectuado com sucesso mas não foi possível obter a fatura. Email não enviado!";
                        return RedirectToPage("/Index");
                    }
                }
                StatusMessage = "O registo de venda foi efectuado com sucesso";
                return RedirectToPage("/Index");
            }
            await SetBasketModelAsync();
            return Page();
        }

        private async Task SetBasketModelAsync()
        {
            var countries = await _countryRepository.ListAllAsync();
            ViewData["Countries"] = new SelectList(countries.OrderBy(x => x.Name), "Id", "Name", "175");
            BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
        }


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

                    if (UserAddress.InvoiceAddressCountry == "175") //Portugal
                    {
                        //Test PostalCode
                        Regex rx = new Regex(@"^\\d{4}-\\d{3}?$");
                        if (!rx.IsMatch(UserAddress.InvoiceAddressPostalCode))
                            ModelState.AddModelError("UserAddress.InvoiceAddressPostalCode", "O campo Código Postal não é válido para Portugal");
                    }
                }
            }
            return ModelState.IsValid;
        }

    }
}
