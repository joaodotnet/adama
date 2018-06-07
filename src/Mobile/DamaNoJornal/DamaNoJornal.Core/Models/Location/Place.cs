using System;
using System.Collections.Generic;
using System.Text;

namespace DamaNoJornal.Core.Models.Location
{
    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }    
}
