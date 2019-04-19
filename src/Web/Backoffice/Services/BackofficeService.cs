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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
        private readonly IInvoiceService _invoiceService;
        private readonly IAuthConfigRepository _authConfigRepository;

        public BackofficeService(DamaContext context,
            IMapper mapper,
            AppIdentityDbContext identityContext,
            IOptions<BackofficeSettings> options,
            IInvoiceService invoiceService,
            IAuthConfigRepository authConfigRepository)
        {
            _damaContext = context;
            _identityContext = identityContext;
            _settings = options.Value;
            _mapper = mapper;
            _invoiceService = invoiceService;
            _authConfigRepository = authConfigRepository;
        }
        public bool CheckIfFileExists(string fullpath, string fileName)
        {
            return System.IO.File.Exists(Path.Combine(fullpath, fileName));
        }

        public void DeleteFile(string fullpath)
        {
            if (System.IO.File.Exists(fullpath))
                System.IO.File.Delete(fullpath);
        }

        public void DeleteFile(string fullpath, string fileName)
        {
            if (System.IO.File.Exists(Path.Combine(fullpath, fileName)))
                System.IO.File.Delete(Path.Combine(fullpath, fileName));
        }

        public PictureInfo SaveFile(IFormFile formFile, string fullPath, string uriPath, string addToFilename, bool resize = false, int width = 0, int height = 0)
        {
            var info = new PictureInfo
            {
                Filename = formFile.GetFileNameSimplify(),
                Extension = formFile.GetExtension()
            };
            var filename = formFile.GetFileName();

            if (!string.IsNullOrEmpty(addToFilename))
            {
                var name = filename.Substring(0, filename.LastIndexOf('.')) + $"-{addToFilename}";
                filename = name + filename.Substring(filename.LastIndexOf('.'));
            }

            //High
            var fileNameHigh = filename.Replace(".", "-high.");
            var fileHighPath = Path.Combine(
                fullPath,
                fileNameHigh);
            using (var stream = new FileStream(fileHighPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            var filePath = Path.Combine(
                fullPath,
                filename);

            //Medium
            if (resize)
            {
                using (Image<Rgba32> image = Image.Load(formFile.OpenReadStream()))
                {
                    image.Mutate(x => x
                         .Resize(width, height));

                    image.Save(filePath); // Automatic encoder selected based on extension.

                    var options = new ResizeOptions
                    {
                        Mode = ResizeMode.Crop,
                        Size = new SixLabors.Primitives.Size(width, height)
                    };

                    image.Mutate(x => x.Resize(options));      

                    image.Save(filePath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 90 }); // Automatic encoder selected based on extension.
                }
            }
            else
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

            }

            info.Location = filePath;
            info.PictureUri = uriPath + filename;
            info.PictureHighUri = uriPath + fileNameHigh;

            return info;
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

            var model = _mapper.Map<List<OrderViewModel>>(orders);

            //check if has customize orders
            foreach (var order in model)
            {
                if (order.HasCustomizeProducts && order.OrderState == OrderStateType.UNDER_ANALYSIS)
                {
                    order.Total = 0;
                }
            }
            return model;
        }

        public async Task<OrderViewModel> GetOrder(int id)
        {
            var order = await _damaContext.Orders
                .Include(x => x.OrderItems)
                //.ThenInclude(i => i.Details)
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.ItemOrdered)
                .SingleOrDefaultAsync(x => x.Id == id);

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            orderViewModel.Items = _mapper.Map<List<OrderItemViewModel>>(order.OrderItems);
            orderViewModel.Items.ForEach(async x => x.ProductSku = await GetSkuAsync(x.ProductId));

            //Attributes
            foreach (var item in orderViewModel.Items)
            {
                //Attributes
                if (item.CatalogAttribute1.HasValue || item.CatalogAttribute2.HasValue || item.CatalogAttribute3.HasValue)
                {
                    var product = await _damaContext.CatalogItems
                    .Include(i => i.CatalogAttributes)
                    .SingleOrDefaultAsync(i => i.Id == item.ProductId);

                    var attributes = new List<CatalogAttribute>();
                    foreach (var attr in product.CatalogAttributes)
                    {
                        if ((item.CatalogAttribute1.HasValue && item.CatalogAttribute1 == attr.Id) ||
                          (item.CatalogAttribute2.HasValue && item.CatalogAttribute2 == attr.Id) ||
                          (item.CatalogAttribute3.HasValue && item.CatalogAttribute3 == attr.Id))
                            attributes.Add(attr);
                    }
                    item.Attributes = attributes.Select(a => new OrderItemAttributeViewModel
                    {
                        AttributeType = a.Type,
                        AttributeName = a.Name
                    }).ToList();
                }
            }


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
            if (type != null)
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
            var response = await _invoiceService.RegisterInvoiceAsync(SageApplicationType.DAMA_BACKOFFICE, order);

            if (response.InvoiceId.HasValue)
            {
                order.SalesInvoiceId = response.InvoiceId.Value;
                order.SalesInvoiceNumber = response.InvoiceNumber;

                await _damaContext.SaveChangesAsync();
            }
            return response;
        }

        public async Task<SageResponseDTO> RegisterPaymentAsync(int id, PaymentType paymentTypeSelected)
        {

            var order = await _damaContext.Orders
                .SingleOrDefaultAsync(x => x.Id == id);

            //Payment
            var response = await _invoiceService.RegisterPaymentAsync(SageApplicationType.DAMA_BACKOFFICE, order.SalesInvoiceId.Value, order.Total(), paymentTypeSelected);

            if (response != null && response.PaymentId.HasValue)
            {
                order.SalesPaymentId = response.PaymentId;
                await _damaContext.SaveChangesAsync();
            }

            return response;
        }

        public async Task<byte[]> GetInvoicePDFAsync(SageApplicationType application, long invoiceId)
        {

            return await _invoiceService.GetPDFInvoiceAsync(application, invoiceId);
        }

        public async Task<byte[]> GetReceiptPDFAsync(long invoiceId, long paymentId)
        {
            return await _invoiceService.GetPDFReceiptAsync(SageApplicationType.DAMA_BACKOFFICE, invoiceId, paymentId);
        }

        public async Task<List<(string, byte[])>> GetOrderDocumentsAsync(int id)
        {
            var invoiceFileName = string.Format(_settings.InvoiceNameFormat, id);
            //var receiptFileName = string.Format(_settings.ReceiptNameFormat, id);
            var invoicePath = Path.Combine(_settings.InvoicesFolderFullPath, invoiceFileName);
            //var receiptPath = Path.Combine(_settings.InvoicesFolderFullPath, receiptFileName);
            List<(string Filename, byte[] Bytes)> files = new List<(string, byte[])>();
            if (File.Exists(invoicePath))
                files.Add((invoiceFileName, await File.ReadAllBytesAsync(invoicePath)));
            //if (File.Exists(receiptPath))
            //    files.Add((receiptFileName, await File.ReadAllBytesAsync(receiptPath)));

            return files;
        }

        //public async Task CreateCatalogPrice(int catalogItemId)
        //{
        //    if (!_damaContext.CatalogPrices.Any(x => x.CatalogItemId == catalogItemId))
        //    {
        //        _damaContext.CatalogPrices.Add(new ApplicationCore.Entities.CatalogPrice
        //        {
        //            CatalogItemId = catalogItemId,
        //            Price = await CheckDefaultPrice(catalogItemId),
        //            Active = true
        //        });
        //        await _damaContext.SaveChangesAsync();
        //    }
        //}        

        //public async Task AddOrUpdateCatalogPrice(int catalogItemId, int? attributeId, decimal? attrPrice)
        //{
        //    if (attributeId.HasValue)
        //    {
        //        //TODO NOT OK
        //        var catalogPrices = await _damaContext.CatalogPrices.Where(x =>
        //            x.CatalogItemId == catalogItemId).ToListAsync();
        //        //(x.Attribute1Id == attributeId || x.Attribute2Id == attributeId || x.Attribute3Id == attributeId));

        //        //var price = await CheckDefaultPrice(catalogItemId) + (attrPrice ?? 0);

        //        if (!catalogPrices.Any(x => x.Attribute1Id == attributeId || x.Attribute2Id == attributeId || x.Attribute3Id == attributeId))
        //        {
        //            foreach (var item in catalogPrices)
        //            {
        //                CatalogPrice newItem = new CatalogPrice
        //                {
        //                    CatalogItemId = item.CatalogItemId,
        //                    Attribute1Id = item.Attribute1Id,
        //                    Attribute2Id = item.Attribute2Id,
        //                    Attribute3Id = item.Attribute3Id,
        //                    Active = true                            
        //                };                         
        //                if (!newItem.Attribute1Id.HasValue)
        //                    newItem.Attribute1Id = attributeId;
        //                else if (!newItem.Attribute2Id.HasValue)
        //                    newItem.Attribute2Id = attributeId;
        //                else if (!newItem.Attribute3Id.HasValue)
        //                    newItem.Attribute3Id = attributeId;
        //                else
        //                    throw new Exception("ATTRIBUTE LIMIT REACH");

        //                //var price = CalculateNewPrice(CheckDefaultPrice(newItem.CatalogItemId), newItem);

        //                _damaContext.CatalogPrices.Add(newItem);
        //            }   
        //        }
        //        else
        //        {
        //            //catalogPrice.Price = price;
        //            //catalogPrice.Active = true;
        //        }
        //    }
        //    else
        //    {
        //        var catalogPrice = await _damaContext.CatalogPrices.Where(x =>
        //            x.CatalogItemId == catalogItemId)
        //            .ToListAsync();
        //        if(catalogPrice != null)
        //        {
        //            foreach (var item in catalogPrice)
        //            {
        //                var price = await CheckDefaultPrice(catalogItemId);
        //                if(item.Attribute1Id.HasValue)
        //                {
        //                    var catalogAttr = await _damaContext.CatalogAttributes
        //                        .FirstOrDefaultAsync(x => x.CatalogItemId == catalogItemId && x.AttributeId == item.Attribute1Id);
        //                    if (catalogAttr != null && catalogAttr.Price.HasValue)
        //                        price += catalogAttr.Price.Value;                           
        //                }
        //                else if(item.Attribute2Id.HasValue)
        //                {
        //                    var catalogAttr = await _damaContext.CatalogAttributes
        //                       .FirstOrDefaultAsync(x => x.CatalogItemId == catalogItemId && x.AttributeId == item.Attribute2Id);
        //                    if (catalogAttr != null && catalogAttr.Price.HasValue)
        //                        price += catalogAttr.Price.Value;                            
        //                }
        //                else if(item.Attribute3Id.HasValue)
        //                {
        //                    var catalogAttr = await _damaContext.CatalogAttributes
        //                        .FirstOrDefaultAsync(x => x.CatalogItemId == catalogItemId && x.AttributeId == item.Attribute3Id);
        //                    if (catalogAttr != null && catalogAttr.Price.HasValue)
        //                        price += catalogAttr.Price.Value;
        //                }
        //                item.Price = price;
        //            }

        //        }
        //    }
        //    await _damaContext.SaveChangesAsync();
        //}

        //public async Task DeleteCatalogPrice(int catalogItemId, int attributeId)
        //{
        //    var catalogPrice = await _damaContext.CatalogPrices.SingleOrDefaultAsync(x =>
        //        x.CatalogItemId == catalogItemId &&
        //        (x.Attribute1Id == attributeId || x.Attribute2Id == attributeId || x.Attribute3Id == attributeId));
        //    if (catalogPrice != null)
        //    {

        //        catalogPrice.Active = false;
        //        await _damaContext.SaveChangesAsync();
        //    }
        //}

        private async Task<decimal> CheckDefaultPrice(int catalogItemId)
        {
            var prod = await _damaContext.CatalogItems
                      .Include(x => x.CatalogType)
                      .SingleOrDefaultAsync(x => x.Id == catalogItemId);

            return prod.Price ?? prod.CatalogType.Price;
        }
    }
}
