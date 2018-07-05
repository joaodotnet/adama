using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CatalogPrice : BaseEntity
    {
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
        public int? CatalogAttribute1Id { get; set; }
        public CatalogAttribute CatalogAttribute1 { get; set; }
        public int? CatalogAttribute2Id { get; set; }
        public CatalogAttribute CatalogAttribute2 { get; set; }
        public int? CatalogAttribute3Id { get; set; }
        public CatalogAttribute CatalogAttribute3 { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
    }
}
