using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SalesWeb.ViewModels;

namespace SalesWeb.Pages.Order
{
    public class DownloadInvoiceModel : PageModel
    {
        private readonly ILogger<DownloadInvoiceModel> _logger;
        private readonly IInvoiceService _invoiceService;

        public DownloadInvoiceModel(
            ILogger<DownloadInvoiceModel> logger, IInvoiceService invoiceService)
        {
            _logger = logger;
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> OnPostAsync(OrderViewModel model)
        {
            var invoiceBytes = await _invoiceService.GetPDFInvoiceAsync(SageApplicationType.SALESWEB, model.InvoiceId);

            Response.Headers.Add("Content-Disposition", $"attachment; filename=SaborComTradicaoFatura#{model.OrderNumber}.pdf");
            return new FileContentResult(invoiceBytes, "application/pdf");
        }
    }
}
