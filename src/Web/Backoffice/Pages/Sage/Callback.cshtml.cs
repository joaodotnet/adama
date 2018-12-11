using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly BackofficeSettings _settings;
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
        public async Task OnGetAsync(string code, string error, string country)
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
                await GetAccessTokenAsync(code);
            }
        }

        private async Task GetAccessTokenAsync(string code)
        {
            var oAuth = await _sageService.GetAccessTokenAsync(ApplicationCore.Entities.SageApplicationType.DAMA_BACKOFFICE, code);

            Status = "You now have access to your Sage One data.";

            AccessToken = oAuth.AccessToken;
            RefreshToken = oAuth.RefreshToken;

            //Save
            await _authRepository.UpdateAuthConfigAsync(ApplicationCore.Entities.SageApplicationType.DAMA_BACKOFFICE, oAuth.AccessToken, oAuth.RefreshToken);
            
        }
    }
}