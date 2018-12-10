using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class OrderViewModel
    {
        [Display(Name = "Nº")]
        public int Id { get; set; }
        [Display(Name = "Email")]
        public string BuyerId { get; set; }
        [Display(Name = "Email Cliente")]
        public string CustomerEmail { get; set; }
        public ApplicationUser User { get; set; }
        [Display(Name = "NIF")]
        public int? TaxNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:yyy-MM-dd HH:mm:ss}")]
        [Display(Name = "Data")]
        public DateTimeOffset OrderDate { get; set; }
        [Display(Name = "Nome (envio)")]
        public string ShipToAddress_Name { get; set; }
        [Display(Name = "Telefone (envio)")]
        public int? ShipToAddress_PhoneNumber { get; set; }
        [Display(Name = "Morada de envio")]
        public string ShipToAddress { get; set; }
        [Display(Name = "Nome (facturação)")]
        public string BillingToAddress_Name { get; set; }
        [Display(Name = "Telefone (facturação)")]
        public int? BillingToAddress_PhoneNumber { get; set; }
        [Display(Name = "Morada de facturação")]
        public string BillingToAddress { get; set; }
        [Display(Name = "Portes")]
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        [Display(Name = "Usar a mesma morada de faturação")]
        public bool UseBillingSameAsShipping { get; set; }
        [Display(Name = "Estado")]
        public OrderStateType OrderState { get; set; }
        [Display(Name = "Nº Produtos")]
        public int ItemsCount { get; set; }
        [Display(Name = "Fatura Nº")]
        public string SalesInvoiceNumber { get; set; }
        public long? SalesInvoiceId { get; set; }
        public long? SalesPaymentId { get; set; }
        public PaymentType PaymentTypeSelected { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public bool HasInvoiceReady { get; set; } = false;
    }
}
