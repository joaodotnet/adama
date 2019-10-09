using ApplicationCore.Entities;
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
        public AddressType AddressType { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public int? TaxNumber { get; set; }
        public string ContactNumber { get; set; }
        public bool? BillingAddressSameAsShipping { get; set; }
        public ApplicationUser User { get; set; }
    }
}
