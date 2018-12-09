using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace SalesWeb.Pages.Sage
{
    public class CallbackModel : PageModel
    {
        private readonly AppSettings _settings;
        private readonly ISageService _sageService;
        private readonly IAuthConfigRepository _authRepository;

        public CallbackModel(IOptions<BackofficeSettings> options, ISageService sageService, IAuthConfigRepository authConfigRepository)
        {
            this._settings = options.Value;
            this._sageService = sageService;
            _authRepository = authConfigRepository;
        }
        public string Status { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public async Task<IActionResult> OnGetAsync(string code, string error, string country)
        {            
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                Status = "An error has occured signing in to Sage One.  <p>Please press back on your browser to start again.";
            }
            else if(!string.IsNullOrEmpty(error))
            {
                Status = $"You denied access to your Sage One data.  Error: {error}";
            }
            else
            {
                if (await GetAccessTokenAsync(code))
                    return RedirectToPage("/Basket/Index");
            }
            return Page();
        }

        private async Task<bool> GetAccessTokenAsync(string code)
        {
            try
            {
                var oAuth = await _sageService.GetAccessTokenAsync(code);

                Status = "You now have access to your Sage One data.";

                AccessToken = oAuth.AccessToken;
                RefreshToken = oAuth.RefreshToken;

                //Save
                await _authRepository.AddOrUpdateAuthConfigAsync(ApplicationCore.Entities.DamaApplicationId.SALESWEB, oAuth.AccessToken, oAuth.RefreshToken);
            }
            catch (SageException ex)
            {
                Status = ex.Message;
                return false;
            }
            return true;
        }
    }
}