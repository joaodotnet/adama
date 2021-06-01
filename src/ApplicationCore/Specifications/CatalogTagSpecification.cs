using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTagSpecification : Specification<CatalogItem>
    {
        public CatalogTagSpecification(string tagName, TagType? tagType, bool showOnlyAvailable) 
        {
            Query
                .Include(x => x.CatalogType)
                .Include(x => x.CatalogIllustration)
                .Include($"{nameof(CatalogItem.CatalogIllustration)}.{nameof(CatalogIllustration.IllustrationType)}")
                .Where(x => x.ShowOnShop &&
                    (!showOnlyAvailable || (showOnlyAvailable && !x.IsUnavailable)) &&
                    (
                    (tagType.HasValue && tagType.Value == TagType.CATALOG_TYPE && x.CatalogType.Slug == tagName) || 
                    (tagType.HasValue && tagType.Value == TagType.ILLUSTRATION && x.CatalogIllustration.Name == tagName) ||
                    (tagType.HasValue && tagType.Value == TagType.ILLUSTRATION_TYPE && x.CatalogIllustration.IllustrationType.Name == tagName) || 
                    (!tagType.HasValue && (x.CatalogType.Slug == tagName || x.CatalogIllustration.Name == tagName || x.CatalogIllustration.IllustrationType.Name == tagName))));
        }
    }
}
