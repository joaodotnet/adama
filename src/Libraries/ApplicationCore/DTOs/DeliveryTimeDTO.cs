using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DTOs
{
    public class DeliveryTimeDTO
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public DeliveryTimeUnitType Unit { get; set; }
    }    
}
