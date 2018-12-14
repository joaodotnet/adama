using ApplicationCore.DTOs;
using ApplicationCore.Entities;
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

        public async Task<(string AccessToken, string RefreshToken)> GenerateNewAccessTokenAsync(SageApplicationType applicationType, string code)
        {
            return await _sageService.GetAccessTokenAsync(applicationType, code);
        }

        public async Task<byte[]> GetPDFInvoiceAsync(SageApplicationType applicationType, long invoiceId)
        {
            return await _sageService.GetPDFInvoice(applicationType, invoiceId);
        }
        public async Task<byte[]> GetPDFReceiptAsync(SageApplicationType applicationType, long invoiceId, long paymentId)
        {
            return await _sageService.GetPDFReceipt(applicationType, invoiceId, paymentId);
        }

        public async Task<SageResponseDTO> RegisterInvoiceAsync(SageApplicationType applicationType, Order order)
        {
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
                response = await _sageService.CreateInvoiceWithTaxNumber(applicationType,
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
                response = await _sageService.CreateAnonymousInvoice(applicationType, items, order.Id, order.ShippingCost);

            return response;
        }

        public async Task<SageResponseDTO> RegisterPaymentAsync(SageApplicationType applicationType, long salesInvoiceId, decimal total, PaymentType paymentType)
        {
            return await _sageService.InvoicePayment(applicationType, salesInvoiceId, paymentType, total);
        }
    }
}
