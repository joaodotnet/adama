using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Interfaces;
using Web.ViewModels;

namespace Web.Pages.Account
{
    public class SigninModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBasketService _basketService;

        public SigninModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IBasketService basketService)
        {
            _signInManager = signInManager;
            _basketService = basketService;
            _userManager = userManager;
        }

        [BindProperty]
        public LoginViewModel LoginDetails { get; set; } = new LoginViewModel();

        [BindProperty]
        public RegisterViewModel UserDetails { get; set; }

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
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            returnUrl = FixBasePath(returnUrl);
            ViewData["ReturnUrl"] = returnUrl;
            //if (!String.IsNullOrEmpty(returnUrl) &&
            //    returnUrl.IndexOf("checkout", StringComparison.OrdinalIgnoreCase) >= 0)
            //{
            //    ViewData["ReturnUrl"] = "/Basket/Index";
            //}
        }

        private string FixBasePath(string returnUrl)
        {        
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.LastIndexOf("/loja") >= 0)
                return returnUrl.Substring(returnUrl.LastIndexOf("/loja") + 5);
            return null;
        }

        public async Task<IActionResult> OnPostSignIn(string returnUrl = null)
        {
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("LoginDetails"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ViewData["ReturnUrl"] = returnUrl;

            var result = await _signInManager.PasswordSignInAsync(LoginDetails.Email, 
                LoginDetails.Password, LoginDetails.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
                if (!String.IsNullOrEmpty(anonymousBasketId))
                {
                    await _basketService.TransferBasketAsync(anonymousBasketId, LoginDetails.Email);
                    Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
                }
                return RedirectToPage(returnUrl ?? "/Index");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = LoginDetails.RememberMe });
            }
            ModelState.AddModelError(string.Empty, "Email ou password inválidos, insira correctamente os dados.");
            return Page();
        }

        public async Task<IActionResult> OnPostRegister(string returnUrl = "/Index")
        {
            foreach (var item in ModelState)
            {
                if (!item.Key.Contains("UserDetails"))
                    item.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = UserDetails.Email, Email = UserDetails.Email, FirstName = UserDetails.FirstName, LastName = UserDetails.LastName, PhoneNumber = UserDetails.PhoneNumber };
                var result = await _userManager.CreateAsync(user, UserDetails.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    string anonymousBasketId = Request.Cookies[Constants.BASKET_COOKIENAME];
                    if (!String.IsNullOrEmpty(anonymousBasketId))
                    {
                        await _basketService.TransferBasketAsync(anonymousBasketId, UserDetails.Email);
                        Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
                    }

                    return LocalRedirect(returnUrl);
                }
                AddErrors(result);
            }
            return Page();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                string description = TryTranslate(error.Description);
                ModelState.AddModelError("", description);
            }
        }

        private string TryTranslate(string description)
        {
            if(description.LastIndexOf("is already taken.") > 0)
            {
                return $"O email '{UserDetails.Email}' já se encontra registado.";
            }
            return description;

        }
    }
}
