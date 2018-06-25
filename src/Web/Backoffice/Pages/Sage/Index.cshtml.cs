using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Backoffice.Pages.Sage
{
    public class IndexModel : PageModel
    {
        private readonly ISageService _sageService;
        private readonly DamaContext _context;
        private readonly IAuthConfigRepository _authConfigRepository;
        private readonly BackofficeSettings _settings;
        public IndexModel(ISageService sageService, IOptions<BackofficeSettings> options, IAuthConfigRepository authConfigRepository, DamaContext context)
        {
            _sageService = sageService;
            _authConfigRepository = authConfigRepository;
            _settings = options.Value;
            _context = context;
        }
        public string Result { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //Check if has auth tokens
            var authTokens = await _authConfigRepository.GetAuthConfigAsync(ApplicationCore.Entities.DamaApplicationId.DAMA_BACKOFFICE);
            if(authTokens == null)
            {
                return Redirect(string.Format(_settings.AuthorizationURL, _settings.ClientId, _settings.CallbackURL));
            }
            return Page();
        }

        public IActionResult OnGetSageAuth()
        {
            return Redirect(string.Format(_settings.AuthorizationURL, _settings.ClientId, _settings.CallbackURL));
        }

        public async Task<IActionResult> OnGetRefreshTokenAsync()
        {
            var tokens = await _sageService.GetAccessTokenByRefreshAsync();
            await _authConfigRepository.AddOrUpdateAuthConfigAsync(ApplicationCore.Entities.DamaApplicationId.DAMA_BACKOFFICE, tokens.AccessToken, tokens.RefreshToken);
            Result = $"Access Token: {tokens.AccessToken} / RefreshToken: {tokens.RefreshToken}";
            return Page();
        }


        public async Task<IActionResult> OnGetAccountDataAsync()
        {
            Result = await _sageService.GetAccountData();
            return Page();
        }

        public async Task<IActionResult> OnGetCreateInvoiceAsync()
        {
            var product = _context.CatalogItems.First();
            var orderItems = new List<OrderItem>
            {
                new OrderItem(new CatalogItemOrdered(product.Id, product.Name, product.PictureUri),product.Price, 1)
            };
            Result = (await _sageService.CreateAnonymousInvoice(orderItems,0)).ResponseBody;
            return Page();
        }

        public async Task<IActionResult> OnPostPaymentInvoiceAsync(int id, decimal amount)
        {
            Result = await _sageService.InvoicePayment(id, amount);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                Result = "Erro. URL não pode ser null";
                return Page();
            }
            Result = await _sageService.GetDataAsync(@url);
            return Page();
        }
    }
}