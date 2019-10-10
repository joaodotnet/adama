using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Interfaces;
using SalesWeb.ViewModels;
using ApplicationCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SalesWeb.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBasketService _basketService;
        private readonly IEmailSender _emailSender;
        private readonly IMailChimpService _mailChimpService;
        private readonly EmailSettings _settings;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IBasketService basketService,
            IEmailSender emailSender,
            IOptions<EmailSettings> settings,
            IMailChimpService mailChimpService,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _basketService = basketService;
            _emailSender = emailSender;
            _mailChimpService = mailChimpService;
            _userManager = userManager;
            _settings = settings.Value;
            _logger = logger;
        }

        [BindProperty]
        public LoginViewModel LoginDetails { get; set; } = new LoginViewModel();

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        //[BindProperty]
        //public RegisterViewModel UserDetails { get; set; } = new RegisterViewModel();

        [TempData]
        public string ErrorMessage { get; set; }

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


        public async Task OnGet(string returnUrl = null)
        {
            //returnUrl = Utils.FixBasePath(returnUrl);
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!String.IsNullOrEmpty(returnUrl) &&
                returnUrl.IndexOf("checkout", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                ViewData["ReturnUrl"] = "/Basket/Index";
            }
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _logger.LogInformation($"OnPostSignIn return url: {returnUrl}");
            if (!ModelState.IsValid)
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }
            

            var result = await _signInManager.PasswordSignInAsync(LoginDetails.Email, 
                LoginDetails.Password, LoginDetails.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
                if (!String.IsNullOrEmpty(anonymousBasketId))
                {
                    await _basketService.TransferBasketAsync(anonymousBasketId, LoginDetails.Email, false);
                    Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
                }                
                return RedirectToPage(returnUrl ?? "/Index");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = LoginDetails.RememberMe });
            }
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ModelState.AddModelError(string.Empty, "Email ou password inválidos, insira correctamente os dados.");
            return Page();
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
                string description = error.Description;
                ModelState.AddModelError("", description);
            }
        }
    }
}
