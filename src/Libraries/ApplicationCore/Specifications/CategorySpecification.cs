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

        public CategorySpecification(bool forMenuList)
            : base(x => x.CatalogCategories.Count > 0)
        {
            AddInclude(x => x.Parent);
            AddInclude(x => x.CatalogCategories);
            AddInclude($"{nameof(Category.CatalogCategories)}.{nameof(CatalogCategory.CatalogItem)}");
            AddInclude(x => x.CatalogTypes);
            AddInclude($"{nameof(Category.CatalogTypes)}.{nameof(CatalogTypeCategory.CatalogType)}");
            ApplyOrderBy(x => x.Order);
        }

        public CategorySpecification(string slug)
            : base(x => x.Slug == slug)
        {
        }
    }
}
