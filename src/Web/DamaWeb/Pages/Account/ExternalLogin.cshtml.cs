using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using DamaWeb.ViewModels.DataAnnotations;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DamaWeb.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IMailChimpService _mailChimpService;
        private readonly CatalogSettings _settings;
        private readonly IBasketService _basketService;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            IOptions<CatalogSettings> settings,
            IMailChimpService mailChimpService,
            IBasketService basketService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _mailChimpService = mailChimpService;
            _settings = settings.Value;
            _basketService = basketService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O {0} é obrigatório.")]
            [EmailAddress(ErrorMessage = "O endereço de Email não é valido.")]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required(ErrorMessage = "O {0} é obrigatório.")]
            [Display(Name = "Nome")]
            public string FirstName { get; set; }
            [Required(ErrorMessage = "O {0} é obrigatório.")]
            [Display(Name = "Apelido")]
            public string LastName { get; set; }
            [Display(Name = "Aceito subscrever a newsletter da Dama no Jornal para ficar a par de todas as novidades.")]
            public bool SubscribeNewsletter { get; set; } = true;
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Signin");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Erro de autenticação externa: {remoteError}";
                return RedirectToPage("./Signin", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Erro ao carregar as informações do login externo.";
                return RedirectToPage("./Signin", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                await TransferBasket(info.Principal.FindFirstValue(ClaimTypes.Email));
                return RedirectToPage(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                Input = new InputModel();
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    Input.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                    Input.FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);

                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                    Input.LastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Erro ao carregar as informações do login externo durante a confirmação";
                return RedirectToPage("./Signin", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                //Check if exists
                var user = await _userManager.FindByEmailAsync(Input.Email);
                IdentityResult result = null;
                if (user == null)
                {
                    user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName };
                    result = await _userManager.CreateAsync(user);
                }

                if (result == null || (result != null &&  result.Succeeded))
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        await TransferBasket(Input.Email);

                        //Check Subscriber
                        if (Input.SubscribeNewsletter)
                        {
                            await _mailChimpService.AddSubscriberAsync(Input.Email);
                            await _emailSender.SendGenericEmailAsync(_settings.FromInfoEmail, _settings.ToEmails, "Subscrição da newsletter feita na loja", $"O utilizador {Input.FirstName} {Input.LastName} registou-se na loja e subscreveu-se na newsletter com o email: {Input.Email}");
                        }

                        return RedirectToPage(returnUrl);


                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }

        private async Task TransferBasket(string email)
        {
            string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
            if (!String.IsNullOrEmpty(anonymousBasketId))
            {
                await _basketService.TransferBasketAsync(anonymousBasketId, email);
                Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
            }
        }
    }
}
