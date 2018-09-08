using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IInvoiceService
    {
        Task<SageResponseDTO> RegisterInvoiceAsync(Order order);
        Task<byte[]> GetPDFInvoiceAsync(long invoiceId);
    }
}
