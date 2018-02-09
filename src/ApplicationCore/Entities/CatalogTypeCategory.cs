using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class CatalogTypeCategory
    {
        public int CatalogTypeId { get; set; }
        public CatalogType CatalogType { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }        
    }
}
