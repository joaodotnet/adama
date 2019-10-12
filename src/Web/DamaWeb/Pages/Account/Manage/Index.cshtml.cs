using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using DamaWeb.ViewModels;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IAccountService _accountService;
        private readonly EmailSettings _settings;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IOptions<EmailSettings> settings,
            IAccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _accountService = accountService;
            _settings = settings.Value;
        }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel UserData { get; set; }

        [BindProperty]
        public AddressViewModel UserShippingAddress { get; set; }

        [BindProperty]
        public InvoiceViewModel UserBillingAddress { get; set; }

        public int? Score { get; set; }

        [BindProperty]
        [Display(Name = "Usar a mesma morada na faturação")]
        public bool CopyInvoiceFromAddress { get; set; }

        public class InputModel
        {
            public string Username { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Telefone")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Nome")]
            public string Firstname { get; set; }
            [Display(Name = "Apelido")]
            public string Lastname { get; set; }
            public int? NIF { get; set; }
            public GenderType? Gender { get; set; }
            public string Fullname
            {
                get
                {
                    if (!string.IsNullOrEmpty(Firstname))
                        return $"{Firstname} {Lastname}";
                    return "";
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UserData = new InputModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                NIF = user.NIF,
                Gender = user.Gender
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            await SetAddressModelAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("UserData"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (UserData.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, UserData.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (UserData.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, UserData.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            if (UserData.Firstname != user.FirstName || UserData.Lastname != user.LastName || UserData.NIF != user.NIF || UserData.Gender != user.Gender)
            {
                user.FirstName = UserData.Firstname;
                user.LastName = UserData.Lastname;
                user.NIF = UserData.NIF;
                user.Gender = UserData.Gender;
                await _userManager.UpdateAsync(user);
            }

            StatusMessage = "O seu perfil foi atualizado.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAddressAsync()
        {
            foreach (var item in ModelState)
            {
                if (CopyInvoiceFromAddress)
                {
                    if (!item.Key.Contains("UserShippingAddress"))
                        item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
                else
                {
                    if (!item.Key.Contains("UserShippingAddress") && !item.Key.Contains("UserBillingAddress"))
                        item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await _accountService.AddOrUpdateUserAddress(user.Id,
                UserShippingAddress.Name,
                null,
                UserShippingAddress.ContactPhoneNumber,
                CopyInvoiceFromAddress,
                UserShippingAddress.Street,
                UserShippingAddress.PostalCode,
                UserShippingAddress.City,
                UserShippingAddress.Country,
                AddressType.SHIPPING);

            if (CopyInvoiceFromAddress)
                await _accountService.AddOrUpdateUserAddress(user.Id,
                    UserShippingAddress.Name,
                    null,
                    UserShippingAddress.ContactPhoneNumber,
                    CopyInvoiceFromAddress,
                    UserShippingAddress.Street,
                    UserShippingAddress.PostalCode,
                    UserShippingAddress.City,
                    UserShippingAddress.Country,
                    AddressType.BILLING);
            else
            {
                await _accountService.AddOrUpdateUserAddress(user.Id,
                    UserBillingAddress.Name,
                    UserBillingAddress.TaxNumber,
                    null,
                    CopyInvoiceFromAddress,
                    UserBillingAddress.AddressStreet,
                    UserBillingAddress.AddressPostalCode,
                    UserBillingAddress.AddressCity,
                    UserBillingAddress.AddressCountry,
                    AddressType.BILLING);
            }

            StatusMessage = "As suas moradas foram atualizadas.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("UserData"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(_settings.FromInfoEmail, user.Email, callbackUrl);

            StatusMessage = "Email de verificação enviado. Por favor verifique o seu email.";
            return RedirectToPage();
        }

        private async Task SetAddressModelAsync(ApplicationUser user)
        {
            var addresses = await _accountService.GetUserAddress(user.Id);
            var shippingAddress = addresses?.SingleOrDefault(x => x.AddressType == AddressType.SHIPPING);
            var billingAddress = addresses?.SingleOrDefault(x => x.AddressType == AddressType.BILLING);
            if (shippingAddress != null)
            {
                UserShippingAddress = new AddressViewModel
                {
                    Name = shippingAddress.Name,
                    TaxNumber = shippingAddress.TaxNumber,
                    ContactPhoneNumber = shippingAddress.ContactNumber,
                    Street = shippingAddress.Street,
                    PostalCode = shippingAddress.PostalCode,
                    City = shippingAddress.City,
                    Country = shippingAddress.Country,
                };
            }
            if (billingAddress != null)
            {
                UserBillingAddress = new InvoiceViewModel
                {
                    Name = billingAddress.Name,
                    TaxNumber = billingAddress.TaxNumber,
                    AddressStreet = billingAddress.Street,
                    AddressPostalCode = billingAddress.PostalCode,
                    AddressCity = billingAddress.City,
                    AddressCountry = billingAddress.Country,
                };
                CopyInvoiceFromAddress = billingAddress.BillingAddressSameAsShipping == true;
            }
        }
    }
}
