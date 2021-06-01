using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeBySlugSpecification : Specification<CatalogType>, ISingleResultSpecification
    {
        public CatalogTypeBySlugSpecification(string slug)
        {
            Query.Where(x => x.Slug == slug);
        }
    }
}
