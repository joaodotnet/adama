using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
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

        public InvoiceService(ISageService sageService)
        {
            _sageService = sageService;
        }

        public async Task<byte[]> GetPDFInvoiceAsync(long invoiceId)
        {
            return await _sageService.GetPDFInvoice(invoiceId);
        }

        public async Task<SageResponseDTO> RegisterInvoiceAsync(Order order)
        {                       
            List<OrderItem> items = new List<OrderItem>();
            foreach (var item in order.OrderItems)
            {
                items.Add(new OrderItem(item.ItemOrdered, item.UnitPrice, item.Units, item.CatalogAttribute1, item.CatalogAttribute2, item.CatalogAttribute3, item.CustomizeName, item.CustomizeSide, null));
            }

            SageResponseDTO response;

            if (order.
                TaxNumber.HasValue)
            {
                var maxStreet1Length = order.BillingToAddress.Street.Length;
                if (maxStreet1Length >= 50)
                    maxStreet1Length = 50;
                response = await _sageService.CreateInvoiceWithTaxNumber(
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
                response = await _sageService.CreateAnonymousInvoice(items, order.Id, order.ShippingCost);

            return response;
        }
    }
}
