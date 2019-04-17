using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using Backoffice.Services;
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
        PictureInfo SaveFile(IFormFile formFile, string fullPath, string uriPath, string addToFileName, bool resize = false, int width = 0, int height = 0);
        Task SaveFileAsync(byte[] bytes, string invoicesFolderFullPath, string fileName);
        void DeleteFile(string fullpath);
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
        Task<byte[]> GetInvoicePDFAsync(SageApplicationType application, long invoiceId);
        Task<SageResponseDTO> RegisterPaymentAsync(int id, PaymentType paymentTypeSelected);
        Task<byte[]> GetReceiptPDFAsync(long invoiceId, long paymentId);
        Task<List<(string,byte[])>> GetOrderDocumentsAsync(int id);
        //Task CreateCatalogPrice(int catalogItemId);
        //Task AddOrUpdateCatalogPrice(int catalogItemId, int? catalogAttributeId, decimal? attrPrice);
        //Task DeleteCatalogPrice(int catalogItemId, int catalogAttributeId);
    }
}
