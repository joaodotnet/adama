using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<ProductType> ProductTypes { get; set; }
    }
}
