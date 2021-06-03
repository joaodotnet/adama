using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeCategorySpec : Specification<CatalogTypeCategory>
    {
        public CatalogTypeCategorySpec(int categoryId)
        {
            Query
                .Include(x => x.CatalogType)
                .Where(x => x.CategoryId == categoryId);
        }
    }
}
