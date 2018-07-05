using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using AutoMapper;
using Backoffice.Extensions;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Services
{
    public class BackofficeService : IBackofficeService
    {
        private readonly DamaContext _damaContext;
        private readonly AppIdentityDbContext _identityContext;
        private readonly BackofficeSettings _settings;
        private readonly IMapper _mapper;
        private readonly ISageService _sageService;

        public BackofficeService(DamaContext context, 
            IMapper mapper, 
            ISageService sageService, 
            AppIdentityDbContext identityContext,
            IOptions<BackofficeSettings> options)
        {
            _damaContext = context;
            _identityContext = identityContext;
            _settings = options.Value;
            _mapper = mapper;
            this._sageService = sageService;
        }
        public bool CheckIfFileExists(string fullpath, string fileName)
        {
            return System.IO.File.Exists(Path.Combine(fullpath,fileName));
        }

        public void DeleteFile(string fullpath, string fileName)
        {
            if (System.IO.File.Exists(Path.Combine(fullpath, fileName)))
                System.IO.File.Delete(Path.Combine(fullpath, fileName));
        }

        public async Task<string> SaveFileAsync(IFormFile formFile, string fullPath, string uriPath, string addToFilename)
        {
            var filename = formFile.GetFileName();

            if (!string.IsNullOrEmpty(addToFilename))
            {                
                var name = filename.Substring(0, filename.LastIndexOf('.')) + $"-{addToFilename}";
                filename = name + filename.Substring(filename.LastIndexOf('.'));
            }

            var filePath = Path.Combine(
                fullPath,
                filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return uriPath + filename; 
        }

        public async Task SaveFileAsync(byte[] bytes, string fullPath, string filename)
        {
            var filePath = Path.Combine(
                fullPath,
                filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

        }

        public async Task<string> GetSku(int typeId, int illustationId, int? attributeId = null)
        {
            var type = await _damaContext.CatalogTypes                
                .Where(x => x.Id == typeId)
                .Select(x => x.Code)
                .SingleAsync();
            var illustration = await _damaContext.CatalogIllustrations
                .Include(x => x.IllustrationType)
                .SingleOrDefaultAsync(x => x.Id == illustationId);
                        
            string sku = $"{type}_{illustration.Code}_{illustration.IllustrationType.Code}";
            if (attributeId.HasValue)
                sku += $"_{attributeId}";
            return sku;
        }

        public async Task<bool> CheckIfSkuExists(string sku)
        {
            return (await _damaContext.CatalogItems.SingleOrDefaultAsync(x => x.Sku == sku)) != null;
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            var orders = await _damaContext.Orders
                .Include(x => x.OrderItems)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
                
            return _mapper.Map<List<OrderViewModel>>(orders);
        }

        public async Task<OrderViewModel> GetOrder(int id)
        {
            var order = await _damaContext.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.Details)
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.ItemOrdered)
                .SingleOrDefaultAsync(x => x.Id == id);

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            orderViewModel.Items = _mapper.Map<List<OrderItemViewModel>>(order.OrderItems);
            orderViewModel.Items.ForEach(async x => x.ProductSku = await GetSkuAsync(x.ProductId));

            //Get user info
            orderViewModel.User = _identityContext.Users.SingleOrDefault(x => x.Email == order.BuyerId);

            return orderViewModel;
        }

        private async Task<string> GetSkuAsync(int catalogItemId)
        {
            var item = await _damaContext.CatalogItems.FindAsync(catalogItemId);
            if (item != null)
                return item.Sku;
            return string.Empty;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync(int productTypeId)
        {
            var type = await _damaContext.CatalogTypes
                .Include(x => x.Categories)
                    .ThenInclude(c => c.Category)
                .SingleOrDefaultAsync(x => x.Id == productTypeId);
            if(type != null)
                return _mapper.Map<List<CategoryViewModel>>(type.Categories.Select(x => x.Category).ToList());
            return null;
        }

        public async Task<IList<CustomizeOrderViewModel>> GetCustomizeOrdersAsync()
        {
            var orders = await _damaContext.CustomizeOrders
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return _mapper.Map<List<CustomizeOrderViewModel>>(orders);
        }

        public async Task<CustomizeOrderViewModel> GetCustomizeOrderAsync(int id)
        {
            var order = await _damaContext.CustomizeOrders.FindAsync(id);
            return _mapper.Map<CustomizeOrderViewModel>(order);
        }

        public async Task<SageResponseDTO> RegisterInvoiceAsync(int id)
        {
            var order = await _damaContext.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.ItemOrdered)
                .SingleOrDefaultAsync(x => x.Id == id);

            List<OrderItem> items = new List<OrderItem>();
            foreach (var item in order.OrderItems)
            {
                items.Add(new OrderItem(item.ItemOrdered, item.UnitPrice, item.Units));
            }

            SageResponseDTO response;
           
            if (order.TaxNumber.HasValue)
            {
                response = await _sageService.CreateInvoiceWithTaxNumber(
                    items,
                    order.BillingToAddress.Name,
                    order.TaxNumber.Value.ToString(),
                    order.BillingToAddress.Street,
                    order.BillingToAddress.PostalCode,
                    order.BillingToAddress.City,
                    order.Id,
                    order.ShippingCost);
            }
            else
                response = await _sageService.CreateAnonymousInvoice(items, order.Id, order.ShippingCost);

            if(response.InvoiceId.HasValue)
            {
                order.SalesInvoiceId = response.InvoiceId.Value;
                order.SalesInvoiceNumber = response.InvoiceNumber;                                

                ////Payment                
                //var responsePayment = await _sageService.InvoicePayment(order.SalesInvoiceId.Value, paymentType, order.Total());

                //if(responsePayment != null && responsePayment.PaymentId.HasValue)
                //{
                //    order.SalesPaymentId = responsePayment.PaymentId;
                //}
                await _damaContext.SaveChangesAsync();
            }
            
            return response;
        }

        public async Task<SageResponseDTO> RegisterPaymentAsync(int id, PaymentType paymentTypeSelected)
        {
            var order = await _damaContext.Orders
                .SingleOrDefaultAsync(x => x.Id == id);

            //Payment
            var response = await _sageService.InvoicePayment(order.SalesInvoiceId.Value, paymentTypeSelected, order.Total());

            if(response != null && response.PaymentId.HasValue)
            {
                order.SalesPaymentId = response.PaymentId;
                await _damaContext.SaveChangesAsync();
            }

            return response;
        }

        public Task<byte[]> GetInvoicePDF(long invoiceId)
        {
            return _sageService.GetPDFInvoice(invoiceId);
        }

        public Task<byte[]> GetReceiptPDF(long invoiceId, long paymentId)
        {
            return _sageService.GetPDFReceipt(invoiceId, paymentId);
        }     
        
        public async Task<List<(string, byte[])>> GetOrderDocumentsAsync(int id)
        {
            var invoiceFileName = string.Format(_settings.InvoiceNameFormat, id);
            //var receiptFileName = string.Format(_settings.ReceiptNameFormat, id);
            var invoicePath = Path.Combine(_settings.InvoicesFolderFullPath, invoiceFileName);
            //var receiptPath = Path.Combine(_settings.InvoicesFolderFullPath, receiptFileName);
            List<(string Filename, byte[] Bytes)> files = new List<(string,byte[])>();
            if (File.Exists(invoicePath))
                files.Add((invoiceFileName, await File.ReadAllBytesAsync(invoicePath)));
            //if (File.Exists(receiptPath))
            //    files.Add((receiptFileName, await File.ReadAllBytesAsync(receiptPath)));

            return files;
        }

        public async Task CreateCatalogPrice(int catalogItemId, decimal price)
        {
            if(!_damaContext.CatalogPrices.Any(x => x.CatalogItemId == catalogItemId))
            {
                _damaContext.CatalogPrices.Add(new ApplicationCore.Entities.CatalogPrice
                {
                    CatalogItemId = catalogItemId,
                    Price = price,
                    Active = true
                });
                await _damaContext.SaveChangesAsync();
            }            
        }

        public async Task AddOrUpdateCatalogPrice(int catalogItemId, int catalogAttributeId, decimal price)
        {
            var catalogPrice = await _damaContext.CatalogPrices.SingleOrDefaultAsync(x =>
                x.CatalogItemId == catalogItemId &&
                (x.CatalogAttribute1Id == catalogAttributeId || x.CatalogAttribute2Id == catalogAttributeId || x.CatalogAttribute3Id == catalogAttributeId));
            if(catalogPrice == null)
            {
                var catalogPrices = _damaContext.CatalogPrices.Where(x => x.CatalogItemId == catalogItemId).ToList();
                catalogPrice = new CatalogPrice { CatalogItemId = catalogItemId, Active = true };
                if (!catalogPrice.CatalogAttribute1Id.HasValue)
                    catalogPrice.CatalogAttribute1Id = catalogAttributeId;
                else if (!catalogPrice.CatalogAttribute2Id.HasValue)
                    catalogPrice.CatalogAttribute2Id = catalogAttributeId;
                else if (!catalogPrice.CatalogAttribute3Id.HasValue)
                    catalogPrice.CatalogAttribute3Id = catalogAttributeId;
                else
                    throw new Exception("ATTRIBUTE LIMIT REACH");

            }
            else
            {
                catalogPrice.Price = price;
                catalogPrice.Active = true;
            }
            await _damaContext.SaveChangesAsync();
        }

        public async Task DeleteCatalogPrice(int catalogItemId, int catalogAttributeId)
        {
            var catalogPrice = await _damaContext.CatalogPrices.SingleOrDefaultAsync(x =>
                x.CatalogItemId == catalogItemId &&
                (x.CatalogAttribute1Id == catalogAttributeId || x.CatalogAttribute2Id == catalogAttributeId || x.CatalogAttribute3Id == catalogAttributeId));
            if (catalogPrice != null)
            {

                catalogPrice.Active = false;
                await _damaContext.SaveChangesAsync();
            }
        }
    }
}
