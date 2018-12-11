using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IInvoiceService
    {
        Task<SageResponseDTO> RegisterInvoiceAsync(SageApplicationType applicationType,Order order);
        Task<SageResponseDTO> RegisterPaymentAsync(SageApplicationType applicationType, long salesInvoiceId, decimal total, PaymentType paymentType);
        Task<byte[]> GetPDFInvoiceAsync(SageApplicationType applicationType, long invoiceId);
    }
}
