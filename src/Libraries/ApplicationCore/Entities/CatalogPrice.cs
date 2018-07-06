using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CatalogPrice : BaseEntity
    {
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
        public int? Attribute1Id { get; set; }
        public Attribute Attribute1 { get; set; }
        public int? Attribute2Id { get; set; }
        public Attribute Attribute2 { get; set; }
        public int? Attribute3Id { get; set; }
        public Attribute Attribute3 { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
    }
}
