using ApplicationCore.Entities;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeSpecification : BaseSpecification<CatalogType>
    {
        public CatalogTypeSpecification(int categoryId)
            :base(x => x.CatalogItems.Count > 0 && x.Categories.Any(c => c.CategoryId == categoryId))
        {
            AddInclude(x => x.CatalogItems);
        }
    }
}
