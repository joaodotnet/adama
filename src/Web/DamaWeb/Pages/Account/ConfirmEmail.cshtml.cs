using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace DamaWeb.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError("Unable to load user with ID '{userId}'.", userId);
                    return RedirectToPage("/Index");
                }

                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (!result.Succeeded)
                {
                    _logger.LogError("Error confirming email for user with ID '{userId}': {messages}", userId, GetErrors(result));
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error in Confirm Email Page");
                return RedirectToPage("/Index");
            }


            return Page();
        }

        private string GetErrors(IdentityResult result)
        {
            string errors = string.Empty;
            if (result.Errors?.Count() > 0)
            {
                foreach (var error in result.Errors)
                {
                    errors += error.Description + ",";
                }
            }
            return errors;
        }
    }
}
