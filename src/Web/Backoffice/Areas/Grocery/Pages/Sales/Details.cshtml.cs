using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using AutoMapper;
using Backoffice.Extensions;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backoffice.Areas.Grocery.Pages.Sales
{
    public class DetailsModel : PageModel
    {        
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _settings;
        private readonly EmailSettings _emailSettings;
        private readonly IEmailSender _emailSender;
        private readonly Infrastructure.Data.GroceryContext _context;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;

        public DetailsModel(IBackofficeService service, 
            IOptions<BackofficeSettings> options,
            IOptions<EmailSettings> emailOptions,
            IEmailSender emailSender,
            Infrastructure.Data.GroceryContext context,
            IMapper mapper,
            IInvoiceService invoiceService)
        {
            _service = service;
            _settings = options.Value;
            _emailSettings = emailOptions.Value;
            _emailSender = emailSender;
            _context = context;
            _mapper = mapper;
            _invoiceService = invoiceService;
        }

        [BindProperty]
        public OrderViewModel OrderModel { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Order order = await GetOrderAsync(id);
            if (order == null)
                return NotFound();
            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            orderViewModel.Items = _mapper.Map<List<OrderItemViewModel>>(order.OrderItems);
            OrderModel = orderViewModel;

            //var fileName = string.Format(_settings.InvoiceNameFormat, id);
            //OrderModel.HasInvoiceReady = _service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName);

            return Page();
        }

        private async Task<Order> GetOrderAsync(int id)
        {
            return await _context.Orders
                            .Include(x => x.OrderItems)
                                .ThenInclude(i => i.ItemOrdered)
                            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var order = await _context.Orders.FindAsync(OrderModel.Id);
                if(order != null)
                {
                    order.OrderState = OrderModel.OrderState;
                    await _context.SaveChangesAsync();
                    StatusMessage = $"O estado da venda  #{OrderModel.Id} foi alterada para {EnumHelper<OrderStateType>.GetDisplayValue(OrderModel.OrderState)}";
                    return RedirectToPage(new { id = OrderModel.Id });
                }
                StatusMessage = "Venda não encontrada!";

            }
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterInvoiceAsync()
        {
            Order order = await GetOrderAsync(OrderModel.Id);
            SageResponseDTO response = await _invoiceService.RegisterInvoiceAsync(SageApplicationType.SALESWEB, order);
            if (response.Message == "Success")
                StatusMessage = $"Sucesso foi criado a fatura: {response.InvoiceNumber}";
            else
                StatusMessage = $"Erro: {response.Message}, Resposta: {response.ResponseBody}";
            return RedirectToPage(new { id = OrderModel.Id });
        }

        public async Task<IActionResult> OnPostRegisterPaymentAsync()
        {
            Order order = await GetOrderAsync(OrderModel.Id);
            SageResponseDTO response = await _invoiceService.RegisterPaymentAsync(SageApplicationType.SALESWEB, order.SalesInvoiceId.Value, order.Total(), OrderModel.PaymentTypeSelected);
            if (response.Message == "Success" && response.PaymentId.HasValue)
            {
                StatusMessage = $"Sucesso foi criado o recibo sobre a fatura {OrderModel.SalesInvoiceNumber}";
                order.SalesPaymentId = response.PaymentId.Value;
                await _context.SaveChangesAsync();
            }                
            else
                StatusMessage = $"Erro: {response.Message}, Resposta: {response.ResponseBody}";

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            orderViewModel.Items = _mapper.Map<List<OrderItemViewModel>>(order.OrderItems);
            OrderModel = orderViewModel;
            return Page();
        }

        public async Task<IActionResult> OnGetInvoicePDFAsync(int id, long invoiceId)
        {
            var fileName = string.Format(_settings.InvoiceGroceryNameFormat,id);
            //Check if file already exist
            if (_service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName))
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(
                    Path.Combine(
                        _settings.InvoicesFolderFullPath,
                        fileName));
                return File(fileBytes, "application/pdf");
            }
            else
            {
                var bytes = await _service.GetInvoicePDFAsync(ApplicationCore.Entities.SageApplicationType.SALESWEB, invoiceId);

                if(bytes.Length > 0)
                    await _service.SaveFileAsync(bytes, _settings.InvoicesFolderFullPath, fileName);

                return File(bytes, "application/pdf",fileName);
            }

        }

        //public async Task<IActionResult> OnGetReceiptPDFAsync(int id, long? invoiceId, long? paymentId)
        //{
        //    if(!invoiceId.HasValue || !paymentId.HasValue)
        //    {
        //        return NotFound();
        //    }
        //    var fileName = string.Format(_settings.ReceiptNameFormat, id);
        //   //Check if file already exist
        //   if (_service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName))
        //   {
        //        byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(
        //           Path.Combine(
        //               _settings.InvoicesFolderFullPath,
        //               fileName));
        //        return File(fileBytes, "application/pdf");
        //   }
        //   else
        //   {
        //       var bytes = await _service.GetReceiptPDFAsync(invoiceId.Value,paymentId.Value);

        //        if(bytes.Length > 0)
        //            await _service.SaveFileAsync(bytes, _settings.InvoicesFolderFullPath, fileName);

        //       return File(bytes, "application/pdf", fileName);
        //   }
        //}

        public async Task<ActionResult> OnPostSendEmailToClient()
        {
            Order order = await GetOrderAsync(OrderModel.Id);
            if (order == null)
                return NotFound();

            var fileName = string.Format(_settings.InvoiceGroceryNameFormat, OrderModel.Id);
            List<(string, byte[])> files = new List<(string, byte[])>();
            //Check if file already exist
            //if (!_service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName))
            //{
            //    files.Add((fileName,await _service.GetInvoicePDF(order.SalesInvoiceId.Value)));
            //}
            if(_service.CheckIfFileExists(_settings.InvoicesFolderFullPath, fileName))
            {
                var invoicePath = Path.Combine(_settings.InvoicesFolderFullPath, fileName);
                files.Add((fileName, await System.IO.File.ReadAllBytesAsync(invoicePath)));
            }
            //var files = await _service.GetOrderDocumentsAsync(order.Id);

            var body = $"<strong>Olá!</strong><br>" +
                            $"Obrigada por comprares na Sabor com Tradição.<br>" +
                            $"Enviamos em anexo a fatura relativa à tua compra. <br>" +
                            "<br>Muito Obrigada.<br>" +
                            "<br>--------------------------------------------------<br>" +
                            "<br><strong>Hi!</strong><br>" +
                            "Thank you to shopping at Sabor Com Tradição in Loulé, Portugal. <br>" +
                            "We send as attach the invoice relates to your purchase.<br>" +
                            "<br>Thank you.<br>" +
                            "<br>Sabor com Tradição" +
                            "<br>http://www.saborcomtradicao.com";

            await _emailSender.SendGenericEmailAsync(_emailSettings.FromOrderEmail, order.CustomerEmail, $"Sabor Com Tradição® - Purchase #{order.Id}", body, _emailSettings.CCEmails, files);
            StatusMessage = "Mensagem Enviada";
            return RedirectToPage(new { id = OrderModel.Id });
        }
    }
}