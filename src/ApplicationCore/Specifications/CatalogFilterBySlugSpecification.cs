using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogFilterBySlugSpecification : Specification<CatalogItem>, ISingleResultSpecification
    {
        public CatalogFilterBySlugSpecification(string slug)
        {
            Query
                .Include(x => x.Categories)
                .Include($"{nameof(CatalogItem.Categories)}.{nameof(CatalogCategory.Category)}")
                .Include(x => x.Pictures)
                .Include(x => x.Attributes)
                .Include(x => x.CatalogType)
                .Include(x => x.CatalogIllustration)
                .Include($"{nameof(CatalogItem.CatalogIllustration)}.{nameof(CatalogIllustration.IllustrationType)}")
                .Include(x => x.CatalogType)
                .Include($"{nameof(CatalogItem.CatalogType)}.{nameof(CatalogType.PictureTextHelpers)}")
                .Include(x => x.References)
                .Include($"{nameof(CatalogItem.References)}.{nameof(CatalogReference.ReferenceCatalogItem)}")
                .Where(x => x.Slug == slug);
        }

    }
}
