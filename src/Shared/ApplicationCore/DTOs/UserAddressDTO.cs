using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Entities;

namespace ApplicationCore.DTOs
{
    public class UserAddressDTO
    {
        //public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public AddressType AddressType { get; set; }
        public string Name { get; set; }
        public bool? BillingAddressSameAsShipping { get; set; }
        public int? TaxNumber { get; set; }
        public string ContactNumber { get; set; }
    }
}
