using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Interfaces
{
    public interface IBackofficeService
    {
        Task<string> SaveFileAsync(IFormFile formFile, string fullPath, string uriPath, string addToFileName);
        Task SaveFileAsync(byte[] bytes, string invoicesFolderFullPath, string fileName);
        void DeleteFile(string fullpath, string fileName);
        bool CheckIfFileExists(string fullpath, string fileName);
        Task<string> GetSku(int typeId, int illustationId, int? attributeId = null);
        Task<bool> CheckIfSkuExists(string sku);
        Task<List<OrderViewModel>> GetOrders();
        Task<OrderViewModel> GetOrder(int id);
        Task<List<CategoryViewModel>> GetCategoriesAsync(int productTypeId);
        Task<IList<CustomizeOrderViewModel>> GetCustomizeOrdersAsync();
        Task<CustomizeOrderViewModel> GetCustomizeOrderAsync(int id);
        Task<SageResponseDTO> RegisterInvoiceAsync(int id);
        Task<byte[]> GetInvoicePDF(long invoiceId);
        Task<SageResponseDTO> RegisterPaymentAsync(int id, PaymentType paymentTypeSelected);
        Task<byte[]> GetReceiptPDF(long invoiceId, long paymentId);
        Task<List<(string,byte[])>> GetOrderDocumentsAsync(int id);
    }
}
