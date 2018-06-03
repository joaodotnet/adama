using System;
using System.Collections.Generic;
using System.Text;

namespace DamaNoJornal.Core.Services.Order.Models
{
    public class AddressEntity
    {
        public String Name { get; set; }
        public int? PhoneNumber { get; set; }
        public String Street { get; set; }
        public String City { get; set; }
        public String Country { get; set; }
        public String PostalCode { get; set; }
    }
}
