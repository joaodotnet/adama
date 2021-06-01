using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogSkuSpecification : Specification<CatalogItem>, ISingleResultSpecification
    {
        public CatalogSkuSpecification(string sku)
        {
            Query.Where(i => i.Sku == sku);
        }
    }
}
