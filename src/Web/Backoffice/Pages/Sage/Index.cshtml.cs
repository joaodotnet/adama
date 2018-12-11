using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
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
        private readonly SageSettings _settings;
        private string _accessToken;
        private string _refreshToken;
        public IndexModel(ISageService sageService, IOptions<SageSettings> options, IAuthConfigRepository authConfigRepository, DamaContext context)
        {
            _sageService = sageService;
            _authConfigRepository = authConfigRepository;
            _settings = options.Value;
            _context = context;
        }
        public string Result { get; set; }
        [TempData]
        public bool GetPDF { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            //Check if has auth tokens
            var authTokens = await _authConfigRepository.GetAuthConfigAsync(ApplicationCore.Entities.SageApplicationType.DAMA_BACKOFFICE);
            if(string.IsNullOrEmpty(authTokens.AccessToken))
            {
                return Redirect(string.Format(_settings.AuthorizationURL, authTokens.ClientId, authTokens.CallbackURL));
            }
            _accessToken = authTokens.AccessToken;
            _refreshToken = authTokens.RefreshToken;
            return Page();
        }

        public async Task<IActionResult> OnGetSageAuthAsync()
        {
            var authTokens = await _authConfigRepository.GetAuthConfigAsync(ApplicationCore.Entities.SageApplicationType.DAMA_BACKOFFICE);
            return Redirect(string.Format(_settings.AuthorizationURL, authTokens.ClientId, authTokens.CallbackURL));
        }

        public async Task<IActionResult> OnGetRefreshTokenAsync()
        {
            var tokens = await _sageService.GetAccessTokenByRefreshAsync(SageApplicationType.DAMA_BACKOFFICE);
            await _authConfigRepository.UpdateAuthConfigAsync(ApplicationCore.Entities.SageApplicationType.DAMA_BACKOFFICE, tokens.AccessToken, tokens.RefreshToken);
            Result = $"Access Token: {tokens.AccessToken} / RefreshToken: {tokens.RefreshToken}";
            return Page();
        }


        public async Task<IActionResult> OnGetAccountDataAsync()
        {
            Result = await _sageService.GetAccountData(SageApplicationType.DAMA_BACKOFFICE);
            return Page();
        }

        //public async Task<IActionResult> OnGetCreateInvoiceAsync()
        //{
        //    var product = _context.CatalogItems.First();
        //    var orderItems = new List<OrderItem>
        //    {
        //        new OrderItem(new CatalogItemOrdered(product.Id, product.Name, product.PictureUri),product.Price.Value, 1, null, null, null, null, null)
        //    };
        //    Result = (await _sageService.CreateAnonymousInvoice(orderItems,0,0)).ResponseBody;
        //    //Result = (await _sageService.CreateInvoiceWithTaxNumber(orderItems, "João Gonçalves","227940032","","","",0)).ResponseBody;
        //    return Page();
        //}

        public async Task<IActionResult> OnPostPaymentInvoiceAsync(int id, decimal amount)
        {
            Result = (await _sageService.InvoicePayment(SageApplicationType.DAMA_BACKOFFICE, id, PaymentType.CASH, amount)).ResponseBody;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                Result = "Erro. URL não pode ser null";
                return Page();
            }
            Result = await _sageService.GetDataAsync(SageApplicationType.DAMA_BACKOFFICE, @url);            
            return Page();
        }

        public async Task<IActionResult> OnPostDownloadPDF(int id)
        {
            var bytes = await _sageService.GetPDFInvoice(SageApplicationType.DAMA_BACKOFFICE, id);
            return File(bytes, "application/pdf",
                $"DamaNoJornal#{id}.pdf");
            
        }
    }
}