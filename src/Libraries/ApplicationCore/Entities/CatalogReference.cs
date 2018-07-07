using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CatalogReference : BaseEntity
    {
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
        public int ReferenceCatalogItemId { get; set; }
        public CatalogItem ReferenceCatalogItem { get; set; }
       
    }
}
