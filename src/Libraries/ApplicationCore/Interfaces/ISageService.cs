using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ISageService
    {
        Task<(string AccessToken, string RefreshToken)> GetAccessTokenAsync(SageApplicationType applicationType,string code);
        Task<SageResponseDTO> CreateAnonymousInvoice(SageApplicationType applicationType, List<OrderItem> orderItems, int referenceId, decimal carriageAmount);
        Task<SageResponseDTO> CreateInvoiceWithTaxNumber(SageApplicationType applicationType, List<OrderItem> orderItems, string customerName, string taxNumber, string address, string address2, string postalCode, string city, string country, int referenceId, decimal carriageAmount);
        Task<string> GetAccountData(SageApplicationType applicationType);
        Task<string> GetDataAsync(SageApplicationType applicationType, string url);
        Task<byte[]> GetPDFInvoice(SageApplicationType applicationType, long id);
        Task<SageResponseDTO> InvoicePayment(SageApplicationType applicationType, long id,PaymentType paymentType, decimal amount);
        Task<(string AccessToken, string RefreshToken)> GetAccessTokenByRefreshAsync(SageApplicationType applicationType);
        Task<byte[]> GetPDFReceipt(SageApplicationType applicationType, long invoiceId, long paymentId);
    }
}