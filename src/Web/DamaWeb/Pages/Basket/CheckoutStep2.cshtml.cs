using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DamaWeb.Pages.Basket
{
    public class CheckoutStep2Model : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _settings;
        private string _username = null;

        public CheckoutStep2Model(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAccountService accountService,
            IOrderService orderService,
            IEmailSender emailSender,
            IOptions<EmailSettings> settings)
        {
            _basketService = basketService;
            _basketViewModelService = basketViewModelService;
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _orderService = orderService;
            _emailSender = emailSender;
            _settings = settings.Value;
        }

        public BasketViewModel BasketModel { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Tens que escolher um método de entrega")]
        public DeliveryMethodType? DeliveryMethod { get; set; }

        [BindProperty]
        public bool WantInvoiceWithTaxNumber { get; set; }

        [BindProperty]
        public bool CopyInvoiceFromAddress { get; set; } = true;

        [BindProperty]
        public AddressViewModel BuyerAddress { get; set; }

        [BindProperty]
        public InvoiceViewModel InvoiceInput { get; set; }

        [BindProperty]
        public StoreViewModel StoreAddress { get; set; }

        [TempData]
        public string GuestEmail { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await SetBasketModelAsync();

            //Check if has any basket item
            if (!BasketModel.Items.Any())
                return RedirectToPage("./Index");

            await SetAddressesModelAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await SetBasketModelAsync();

            if (!DeliveryMethod.HasValue)
                return Page();

            FormValidation();

            if (ModelState.IsValid)
            {
                await SaveUserDataAsync();
                await SaveAddressesAsync();

                //Preparing Order
                string buyerName = DeliveryMethod == DeliveryMethodType.HOME ? BuyerAddress.Name : StoreAddress.Name;
                decimal shippingcost = DeliveryMethod == DeliveryMethodType.HOME ? BasketModel.DefaultShippingCost : 0;
                string phoneNumber = DeliveryMethod == DeliveryMethodType.HOME ? BuyerAddress.ContactPhoneNumber : StoreAddress.ContactPhone;
                int? taxNumber = DeliveryMethod == DeliveryMethodType.HOME && CopyInvoiceFromAddress ? BuyerAddress.TaxNumber : InvoiceInput.TaxNumber;
                var shippingAddress = DeliveryMethod == DeliveryMethodType.HOME ?
                    new Address(BuyerAddress.Name,BuyerAddress.Street, BuyerAddress.City, BuyerAddress.Country, BuyerAddress.PostalCode) :
                    new Address(StoreAddress.Name, "Mercado de Loulé - Banca nº 44, Praça da Republica", "Loulé", "Portugal", "8100-270");
                var billingAddress = DeliveryMethod == DeliveryMethodType.HOME && CopyInvoiceFromAddress ?
                    new Address(BuyerAddress.Name, BuyerAddress.Street, BuyerAddress.City, BuyerAddress.Country, BuyerAddress.PostalCode) :
                    new Address(InvoiceInput.Name, InvoiceInput.AddressStreet, InvoiceInput.AddressCity, InvoiceInput.AddressCountry, InvoiceInput.AddressPostalCode);
                
                //Create Order
                var resOrder = await _orderService.CreateOrderAsync(BasketModel.Id, 
                    phoneNumber, 
                    WantInvoiceWithTaxNumber ? taxNumber : default, 
                    shippingAddress, 
                    WantInvoiceWithTaxNumber ? billingAddress: null,
                    WantInvoiceWithTaxNumber && DeliveryMethod == DeliveryMethodType.HOME ? CopyInvoiceFromAddress : false, 
                    shippingcost);

                var deliveryTime = await _basketService.CalculateDeliveryTime(BasketModel.Id);
                var orderAttributes = await _orderService.GetOrderAttributesAsync(resOrder.OrderItems.ToList()); //Check Order Items

                await _emailSender.SendCreateOrderEmailAsync(
                    _settings.FromOrderEmail,
                    _settings.CCEmails,
                    buyerName,
                    resOrder,
                    orderAttributes,
                    DeliveryMethod == DeliveryMethodType.STORE,
                    deliveryTime,
                    WantInvoiceWithTaxNumber);

                await _basketService.DeleteBasketAsync(BasketModel.Id);

                return RedirectToPage("./Result", new { id = resOrder.Id });
            }

            return Page();
        }

        private async Task SaveUserDataAsync()
        {
            if(BuyerAddress.SaveAddress && _signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!string.IsNullOrEmpty(BuyerAddress.Name))
                {
                    user.FirstName = BuyerAddress.Name.IndexOf(" ") > 0 ? BuyerAddress.Name.Substring(0, BuyerAddress.Name.IndexOf(" ")) : BuyerAddress.Name;
                    if (BuyerAddress.Name.IndexOf(" ") > 0)
                        user.LastName = BuyerAddress.Name.Substring(BuyerAddress.Name.LastIndexOf(" "));
                }
                user.NIF = BuyerAddress.TaxNumber;
                user.PhoneNumber = BuyerAddress.ContactPhoneNumber;
                await _userManager.UpdateAsync(user);
            }
        }

        private async Task SaveAddressesAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (DeliveryMethod == DeliveryMethodType.HOME && BuyerAddress.SaveAddress)
                {
                    await _accountService.AddOrUpdateUserAddress(user.Id, BuyerAddress.Name, BuyerAddress.TaxNumber, BuyerAddress.ContactPhoneNumber, WantInvoiceWithTaxNumber && CopyInvoiceFromAddress, BuyerAddress.Street, BuyerAddress.PostalCode, BuyerAddress.City, BuyerAddress.Country, AddressType.SHIPPING);

                    if (WantInvoiceWithTaxNumber && CopyInvoiceFromAddress)
                        await _accountService.AddOrUpdateUserAddress(user.Id, BuyerAddress.Name, BuyerAddress.TaxNumber, BuyerAddress.ContactPhoneNumber, true, BuyerAddress.Street, BuyerAddress.PostalCode, BuyerAddress.City, BuyerAddress.Country, AddressType.BILLING);
                }

                if ((DeliveryMethod == DeliveryMethodType.HOME && WantInvoiceWithTaxNumber && !CopyInvoiceFromAddress && InvoiceInput.InvoiceSaveAddress) ||
                    (DeliveryMethod == DeliveryMethodType.STORE && WantInvoiceWithTaxNumber && InvoiceInput.InvoiceSaveAddress))
                    await _accountService.AddOrUpdateUserAddress(user.Id, InvoiceInput.Name, InvoiceInput.TaxNumber, DeliveryMethod == DeliveryMethodType.HOME ? BuyerAddress.ContactPhoneNumber : StoreAddress.ContactPhone, false, InvoiceInput.AddressStreet, InvoiceInput.AddressPostalCode, InvoiceInput.AddressCity, InvoiceInput.AddressCountry, AddressType.BILLING);
            }
        }

        private async Task SetAddressesModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var addresses = await _accountService.GetUserAddress(user.Id);
                    var shippingAddress = addresses?.SingleOrDefault(x => x.AddressType == AddressType.SHIPPING);
                    var billingAddress = addresses?.SingleOrDefault(x => x.AddressType == AddressType.BILLING);
                    if (shippingAddress != null)
                    {
                        BuyerAddress = new AddressViewModel
                        {
                            Name = shippingAddress.Name,
                            TaxNumber = shippingAddress.TaxNumber,
                            ContactPhoneNumber = shippingAddress.ContactNumber,
                            Street = shippingAddress.Street,
                            PostalCode = shippingAddress.PostalCode,
                            City = shippingAddress.City,
                            Country = shippingAddress.Country,
                            SaveAddress = true
                        };
                    }
                    if (billingAddress != null)
                    {
                        InvoiceInput = new InvoiceViewModel
                        {
                            Name = billingAddress.Name,
                            TaxNumber = billingAddress.TaxNumber,
                            AddressStreet = billingAddress.Street,
                            AddressPostalCode = billingAddress.PostalCode,
                            AddressCity = billingAddress.City,
                            AddressCountry = billingAddress.Country,
                            InvoiceSaveAddress = true
                        };
                    }
                }
            }
        }

        private void FormValidation()
        {
            foreach (var item in ModelState)
            {
                if (DeliveryMethod.Value == DeliveryMethodType.HOME)
                {
                    if (WantInvoiceWithTaxNumber && CopyInvoiceFromAddress)
                    {
                        if (!item.Key.Contains("BuyerAddress"))
                            item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                        if (!BuyerAddress.TaxNumber.HasValue)
                            ModelState.AddModelError("BuyerAddress.TaxNumber", "O campo NIF é obrigatório");
                    }
                    else if (WantInvoiceWithTaxNumber && !CopyInvoiceFromAddress)
                    {
                        if (!item.Key.Contains("BuyerAddress") && !item.Key.Contains("InvoiceInput"))
                            item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }
                    else if (!WantInvoiceWithTaxNumber)
                    {
                        if (!item.Key.Contains("BuyerAddress"))
                            item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }
                }
                else
                {
                    if (WantInvoiceWithTaxNumber)
                    {
                        if (!item.Key.Contains("StoreAddress") && !item.Key.Contains("InvoiceInput"))
                            item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }
                    else
                    {
                        if (!item.Key.Contains("StoreAddress"))
                            item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }

                }
            }
        }

        private async Task SetBasketModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else if(!string.IsNullOrEmpty(GuestEmail))
            {
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(GuestEmail);                
            }
            else
            {
                GetOrSetBasketCookieAndUserName();
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }

            //Set Min and Max Delivery time
            if (!BasketModel.HasCustomizeItems)
                BasketModel.DeliveryTime = await _basketService.CalculateDeliveryTime(BasketModel.Id);

            BasketModel.CanSubmit = true;
            BasketModel.ShowShippingCost = DeliveryMethod == DeliveryMethodType.HOME;
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
