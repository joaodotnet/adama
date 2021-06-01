using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogAttrFilterSpecification : Specification<CatalogItem>, ISingleResultSpecification
    {
        public CatalogAttrFilterSpecification(int catalogId)
        {
            Query
                .Include(x => x.Attributes)
                .Where(i => i.Id == catalogId);
        }
    }
}
