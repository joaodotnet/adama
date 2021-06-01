using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeFilterSpecification : Specification<CatalogItem>, ISingleResultSpecification
    {
        public CatalogTypeFilterSpecification(int catalogId)
        {
            Query
            .Include(x => x.CatalogType)
            .Include(x => x.Attributes)
            .Where(i => i.Id == catalogId);
        }
    }
}
