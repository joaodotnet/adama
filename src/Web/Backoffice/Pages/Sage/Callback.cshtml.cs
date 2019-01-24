using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly BackofficeSettings _settings;
        private readonly IInvoiceService _invoiceService;
        private readonly IAuthConfigRepository _authRepository;

        public CallbackModel(IOptions<BackofficeSettings> options, IInvoiceService invoiceService, IAuthConfigRepository authConfigRepository)
        {
            _settings = options.Value;
            _invoiceService = invoiceService;
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
            var oAuth = await _invoiceService.GenerateNewAccessTokenAsync(SageApplicationType.DAMA_BACKOFFICE, code);
            Status = "You now have access to your Sage One data.";

            AccessToken = oAuth.AccessToken;
            RefreshToken = oAuth.RefreshToken;

            //Save
            await _authRepository.UpdateAuthConfigAsync(ApplicationCore.Entities.SageApplicationType.DAMA_BACKOFFICE, oAuth.AccessToken, oAuth.RefreshToken);
            
        }
    }
}