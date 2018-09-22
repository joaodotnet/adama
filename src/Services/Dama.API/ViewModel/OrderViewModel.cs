using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dama.API.ViewModel
{
    public class OrderViewModel
    {
        [Required]
        public string BuyerId { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingStreet { get; set; }        
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }

        public string ShippingZipCode { get; set; }
        public string BillingName { get; set; }
        public string BillingCity { get; set; }

        public string BillingStreet { get; set; }

        public string BillingCountry { get; set; }

        public string BillingPostalCode { get; set; }
        public string CardNumber { get; set; }
        
        public string CardHolderName { get; set; }
        
        public DateTime CardExpiration { get; set; }
        
        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }
        public string CustomerEmail { get; set; }
        public int? TaxNumber { get; set; }
        public bool CreateInvoice { get; set; }
        public string ResultMessage { get; set; }
        public int id { get; set; }
    }


}
