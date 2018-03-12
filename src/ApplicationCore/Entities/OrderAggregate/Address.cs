using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities.OrderAggregate
{
    public class Address // ValueObject
    {
        public String Name { get; private set; }
        public int? PhoneNumber { get; set; }
        public String Street { get; private set; }

        public String City { get; private set; }

        public String Country { get; private set; }

        public String PostalCode { get; private set; }       

        public Address() { }

        public Address(string name, int? phoneNumber, string street, string city, string country, string postalCode) 
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Street = street;
            City = city;
            Country = country;
            PostalCode = postalCode;
        }
    }
}
