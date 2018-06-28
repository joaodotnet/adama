using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ISageService
    {
        Task<(string AccessToken, string RefreshToken)> GetAccessTokenAsync(string code);
        Task<SageResponseDTO> CreateAnonymousInvoice(List<OrderItem> orderItems, int referenceId);
        Task<SageResponseDTO> CreateInvoiceWithTaxNumber(List<OrderItem> orderItems, string customerName, string taxNumber, string address, string postalCode, string city, int referenceId);
        Task<string> GetAccountData();
        Task<string> GetDataAsync(string url);
        Task<byte[]> GetPDFInvoice(long id);
        Task<SageResponseDTO> InvoicePayment(long id,PaymentType paymentType, decimal amount);
        Task<(string AccessToken, string RefreshToken)> GetAccessTokenByRefreshAsync();
        Task<byte[]> GetPDFReceipt(long invoiceId, long paymentId);
    }
}