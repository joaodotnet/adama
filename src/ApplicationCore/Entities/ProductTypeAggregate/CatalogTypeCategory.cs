using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class CatalogTypeCategory
    {

        public int CatalogTypeId { get; private set; }
        public CatalogType CatalogType { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; }        
        
        public CatalogTypeCategory(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
