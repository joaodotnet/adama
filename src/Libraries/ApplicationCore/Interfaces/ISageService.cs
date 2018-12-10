using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ISageService
    {
        Task<(string AccessToken, string RefreshToken)> GetAccessTokenAsync(string code);
        Task<SageResponseDTO> CreateAnonymousInvoice(string accessToken, string refreshToken, List<OrderItem> orderItems, int referenceId, decimal carriageAmount);
        Task<SageResponseDTO> CreateInvoiceWithTaxNumber(string accessToken, string refreshToken, List<OrderItem> orderItems, string customerName, string taxNumber, string address, string address2, string postalCode, string city, int referenceId, decimal carriageAmount);
        Task<string> GetAccountData(string accessToken, string refreshToken);
        Task<string> GetDataAsync(string accessToken, string refreshToken, string url);
        Task<byte[]> GetPDFInvoice(string accessToken, string refreshToken, long id);
        Task<SageResponseDTO> InvoicePayment(string accessToken, string refreshToken, long id,PaymentType paymentType, decimal amount);
        Task<(string AccessToken, string RefreshToken)> GetAccessTokenByRefreshAsync(string refreshToken);
        Task<byte[]> GetPDFReceipt(string accessToken, string refreshToken, long invoiceId, long paymentId);
    }
}