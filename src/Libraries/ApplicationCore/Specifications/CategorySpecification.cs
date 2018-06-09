using ApplicationCore.Entities;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification()
            :base(x => !x.ParentId.HasValue && x.CatalogCategories.Count > 0 && x.CatalogCategories.Any(c => c.CatalogItem.CanCustomize))
        {
            AddInclude(x => x.CatalogCategories);
        }
    }
}
