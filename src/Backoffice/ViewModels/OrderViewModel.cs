using ApplicationCore.Entities.OrderAggregate;
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
        [Display(Name = "NIF")]
        public int? TaxNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:yyy-MM-dd HH:mm:ss}")]
        [Display(Name = "Data")]
        public DateTimeOffset OrderDate { get; set; }
        public string ShipToAddress_Name { get; set; }
        public int? ShipToAddress_PhoneNumber { get; set; }
        [Display(Name = "Morada")]
        public string ShipToAddress { get; set; }
        public string BillingToAddress_Name { get; set; }
        public int? BillingToAddress_PhoneNumber { get; set; }
        public string BillingToAddress { get; set; }
        [Display(Name = "Portes")]
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public bool UseBillingSameAsShipping { get; set; }
        [Display(Name = "Estado")]
        public OrderStateType OrderState { get; set; }
        [Display(Name = "Nº Produtos")]
        public int ItemsCount { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }
}
