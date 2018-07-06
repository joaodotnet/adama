using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Attribute : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        public AttributeType Type { get; set; }

        public virtual ICollection<CatalogAttribute> CatalogAttributes { get; set; }
        //public virtual ICollection<CatalogPrice> CatalogPrices { get; set; }
    }
}
