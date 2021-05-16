using ApplicationCore.Entities;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeSpecification : BaseSpecification<CatalogType>
    {
        public CatalogTypeSpecification(int categoryId)
            :base(x => x.Categories.Any(c => c.CategoryId == categoryId))
        {
            AddInclude(x => x.CatalogItems);
        }

        public CatalogTypeSpecification(bool includeHelpers)
            : base(x => true)
        {
            if(includeHelpers)
                AddInclude(x => x.PictureTextHelpers);
        }

        public CatalogTypeSpecification(string slug)
            : base(x => x.Slug == slug)
        {
        }
    }
}
