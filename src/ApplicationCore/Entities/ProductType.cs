using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductType : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
