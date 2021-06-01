using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CategorySpecification : Specification<Category>
    {
        public CategorySpecification()
        {
            Query
                .Include(x => x.CatalogCategories)
                .Where(x => !x.ParentId.HasValue && x.CatalogCategories.Any(c => c.CatalogItem.CanCustomize));
        }

        public CategorySpecification(bool forMenuList)
        {
            Query
                .Include(x => x.Parent)
                .Include(x => x.CatalogCategories)
                .Include($"{nameof(Category.CatalogCategories)}.{nameof(CatalogCategory.CatalogItem)}")
                .Include(x => x.CatalogTypes)
                .Include($"{nameof(Category.CatalogTypes)}.{nameof(CatalogTypeCategory.CatalogType)}")
                .Where(x => x.CatalogCategories.Any())
                .OrderBy(x => x.Order);
        }
    }
}
