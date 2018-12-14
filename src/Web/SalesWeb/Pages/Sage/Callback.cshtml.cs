using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Entities;
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
                var oAuth = await _invoiceService.GenerateNewAccessTokenAsync(SageApplicationType.SALESWEB, code);

                Status = "You now have access to your Sage One data.";

                AccessToken = oAuth.AccessToken;
                RefreshToken = oAuth.RefreshToken;

                //Save
                await _authRepository.UpdateAuthConfigAsync(SageApplicationType.SALESWEB, oAuth.AccessToken, oAuth.RefreshToken);
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