using System;
using System.Collections.Generic;

namespace Infrastructure.Identity
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }        
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public bool DefaultAddress { get; set; }        
        public bool UseInvoiceSameAsShipping { get; set; }
        public bool IsInvoiceAddress { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
