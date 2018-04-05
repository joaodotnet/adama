using ApplicationCore.Entities;

namespace ApplicationCore.Specifications
{
    public class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification()
            :base(x => !x.ParentId.HasValue && x.CatalogCategories.Count > 0)
        {
            AddInclude(x => x.CatalogCategories);
        }
    }
}
