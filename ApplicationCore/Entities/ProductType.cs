using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductType : BaseEntity
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }
}
