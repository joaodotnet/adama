using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ISageService _sageService;
        private readonly IAuthConfigRepository _authConfigRepository;
        private readonly SageSettings _settings;

        public InvoiceService(ISageService sageService, 
            IAuthConfigRepository authConfigRepository,
            IOptions<SageSettings> settings)
        {
            _sageService = sageService;
            _authConfigRepository = authConfigRepository;
            _settings = settings.Value;
        }

        public async Task<byte[]> GetPDFInvoiceAsync(long invoiceId)
        {
            //Get Tokens
            var tokens = await GetTokensAsync();
            if (tokens == null)
                return null;

            return await _sageService.GetPDFInvoice(tokens.AccessToken, tokens.RefreshToken, invoiceId);
        }

        public async Task<SageResponseDTO> RegisterInvoiceAsync(Order order)
        {
            //Get Tokens
            var tokens = await GetTokensAsync();
            if (tokens == null)
                return new SageResponseDTO { Message = "Erro: Configuração de acesso à Sage inexistente!" };

            List<OrderItem> items = new List<OrderItem>();
            foreach (var item in order.OrderItems)
            {
                items.Add(new OrderItem(item.ItemOrdered, item.UnitPrice, item.Units, item.CatalogAttribute1, item.CatalogAttribute2, item.CatalogAttribute3, item.CustomizeName, item.CustomizeSide));
            }

            SageResponseDTO response;

            if (order.TaxNumber.HasValue)
            {
                var maxStreet1Length = order.BillingToAddress.Street.Length;
                if (maxStreet1Length >= 50)
                    maxStreet1Length = 50;
                response = await _sageService.CreateInvoiceWithTaxNumber(tokens.AccessToken,
                    tokens.RefreshToken,
                    items,
                    order.BillingToAddress.Name,
                    order.TaxNumber.Value.ToString(),
                    order.BillingToAddress.Street.Substring(0, maxStreet1Length),
                    order.BillingToAddress.Street.Length > 50 ? order.BillingToAddress.Street.Substring(50) : string.Empty,
                    order.BillingToAddress.PostalCode,
                    order.BillingToAddress.City,
                    order.Id,
                    order.ShippingCost);
            }
            else
                response = await _sageService.CreateAnonymousInvoice(tokens.AccessToken, tokens.RefreshToken, items, order.Id, order.ShippingCost);

            return response;
        }

        public async Task<SageResponseDTO> RegisterPaymentAsync(long salesInvoiceId, decimal total, PaymentType paymentType)
        {
            var tokens = await GetTokensAsync();
            if (tokens == null)
                return new SageResponseDTO { Message = "Erro: Configuração de acesso à Sage inexistente!" };

            return await _sageService.InvoicePayment(tokens.AccessToken, tokens.RefreshToken, salesInvoiceId, paymentType, total);
        }

        private async Task<TokenKeys> GetTokensAsync()
        {
            var tokens = await _authConfigRepository.GetAuthConfigAsync(_settings.ClientApp);
            if (tokens != null)
                return new TokenKeys { AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken };
            return null;
        }
        private class TokenKeys
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
