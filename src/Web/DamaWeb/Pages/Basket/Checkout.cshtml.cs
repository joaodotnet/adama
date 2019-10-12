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
using ApplicationCore;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.ApplicationInsights;
using System.ComponentModel.DataAnnotations;
using DamaWeb.ViewModels.DataAnnotations;

namespace DamaWeb.Pages.Basket
{
    public class CheckoutModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private string _username = null;
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IEmailSender _emailSender;
        private readonly IMailChimpService _mailChimpService;
        private readonly EmailSettings _settings;

        public CheckoutModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IOptions<EmailSettings> settings,
            IMailChimpService mailChimpService)
        {
            _basketService = basketService;
            _userManager = userManager;
            _signInManager = signInManager;
            _basketViewModelService = basketViewModelService;
            _emailSender = emailSender;
            _mailChimpService = mailChimpService;
            _settings = settings.Value;

        }
        public BasketViewModel BasketModel { get; set; }

        [BindProperty]
        public LoginViewModel LoginModel { get; set; }

        [BindProperty]
        public RegisterViewModel RegisterModel { get; set; }

        [BindProperty]
        public GuestViewModel GuestModel { get; set; }

        public class LoginViewModel
        {
            [Required(ErrorMessage = "O endereço de Email é obrigatório.")]
            [EmailAddress(ErrorMessage = "O endereço de Email não é valido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A {0} é obrigatória.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Memorizar?")]
            public bool RememberMe { get; set; }
        }

        public class GuestViewModel
        {
            [Required(ErrorMessage = "O endereço de Email é obrigatório.")]
            [EmailAddress(ErrorMessage = "O endereço de Email não é valido.")]
            public string Email { get; set; }
            [Display(Name = "Aceito subscrever a newsletter da Dama no Jornal para ficar a par de todas as novidades.")]
            public bool SubscribeNewsletter { get; set; } = true;
            [EnforceTrue(ErrorMessage = "Deves aceitar os Termos e Condições.")]
            public bool AgreeToTerms { get; set; } = false;
        }

        public async Task<IActionResult> OnGet()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToPage("./CheckoutStep2");

            await SetBasketModelAsync();

            //Check if has any basket item
            if (!BasketModel.Items.Any())
                return RedirectToPage("./Index");

            return Page();
        }

        public async Task<IActionResult> OnPostSignInAsync([FromBody] LoginViewModel model)
        {
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("LoginModel"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(LoginModel.Email,
                LoginModel.Password, LoginModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
                    if (!string.IsNullOrEmpty(anonymousBasketId))
                    {
                        await _basketService.TransferBasketAsync(anonymousBasketId, LoginModel.Email, false);
                        Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
                    }
                    return RedirectToPage("CheckoutStep2");
                }
                ModelState.AddModelError(string.Empty, "Email ou password inválidos, insira correctamente os dados.");
            }

            //Something went wrong
            await SetBasketModelAsync();
            ViewData["LoginFailed"] = true;
            return Page();
        }

        public async Task<IActionResult> OnPostRegister()
        {            
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("RegisterModel"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = RegisterModel.Email,
                    Email = RegisterModel.Email,
                    FirstName = RegisterModel.FirstName,
                    LastName = RegisterModel.LastName,
                    PhoneNumber = RegisterModel.PhoneNumber,
                    Gender = RegisterModel.Gender
                };
                var result = await _userManager.CreateAsync(user, RegisterModel.Password);
                if (result.Succeeded)
                {
                    string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
                    if (!string.IsNullOrEmpty(anonymousBasketId))
                    {
                        await _basketService.TransferBasketAsync(anonymousBasketId, RegisterModel.Email, false);
                        Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
                    }

                    await SendConfirmationEmailAsync(user);

                    //Check Subscriber
                    if (RegisterModel.SubscribeNewsletter)
                    {
                        await _mailChimpService.AddSubscriberAsync(RegisterModel.Email);
                        await _emailSender.SendGenericEmailAsync(_settings.FromInfoEmail, _settings.CCEmails, "Subscrição da newsletter feita na loja", $"O utilizador {RegisterModel.FirstName} {RegisterModel.LastName} registou-se na loja e subscreveu-se na newsletter com o email: {RegisterModel.Email}");
                    }
                    else
                        await _emailSender.SendGenericEmailAsync(_settings.FromInfoEmail, _settings.CCEmails, "Novo registo na loja", $"O utilizador {RegisterModel.FirstName} {RegisterModel.LastName} registou-se na loja com o email: {RegisterModel.Email}");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("CheckoutStep2");
                }
                AddErrors(result);
            }

            //Something went wrong
            await SetBasketModelAsync();
            ViewData["RegisterFailed"] = true;
            return Page();
        }

        public async Task<IActionResult> OnPostLogInAsGuest()
        {
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("GuestModel"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
                if (!string.IsNullOrEmpty(anonymousBasketId))
                {
                    await _basketService.TransferBasketAsync(anonymousBasketId, GuestModel.Email, true);
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.Today.AddYears(1),
                        IsEssential = true
                    };
                    Response.Cookies.Append(Constants.BASKET_COOKIENAME, GuestModel.Email, cookieOptions);
                }

                //Check Subscriber
                if (GuestModel.SubscribeNewsletter)
                {
                    await _mailChimpService.AddSubscriberAsync(GuestModel.Email);
                    await _emailSender.SendGenericEmailAsync(_settings.FromInfoEmail, _settings.CCEmails, "Subscrição da newsletter feita na loja", $"Um convidado anónimo subscreveu-se na newsletter com o email: {GuestModel.Email}");
                }
                TempData["GuestEmail"] = GuestModel.Email;
                return RedirectToPage("./CheckoutStep2");
            }
            //Something went wrong
            await SetBasketModelAsync();
            ViewData["GuestFailed"] = true;
            return Page();
        }

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

            //Set Min and Max Delivery time
            if (!BasketModel.HasCustomizeItems)
                BasketModel.DeliveryTime = await _basketService.CalculateDeliveryTime(BasketModel.Id);
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

        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(_settings.FromInfoEmail, user.Email, callbackUrl);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                string description = Utils.TryTranslate(error.Description, RegisterModel.Email);
                ModelState.AddModelError("", description);
            }
        }
    }   
}
