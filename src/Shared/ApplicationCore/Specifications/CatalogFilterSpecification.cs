using ApplicationCore.Entities;
using System.Linq;

namespace ApplicationCore.Specifications
{

    public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogFilterSpecification(int? IllustrationId, int? typeId, int? categoryId, bool? canCustomize = null, bool? showOnlyAvailable = null)
            : base(i => i.ShowOnShop &&
            (!IllustrationId.HasValue || i.CatalogIllustrationId == IllustrationId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId) &&
            (!categoryId.HasValue || i.CatalogCategories.Any(x => x.CategoryId == categoryId)) &&
            (!canCustomize.HasValue || i.CanCustomize == canCustomize.Value) &&
            (!showOnlyAvailable.HasValue || (showOnlyAvailable.HasValue && !showOnlyAvailable.Value) || (showOnlyAvailable.HasValue && showOnlyAvailable.Value && !i.IsUnavailable)))
        {
            AddInclude(x => x.CatalogType);
            AddInclude(x => x.CatalogCategories);
        }

        public CatalogFilterSpecification(bool onlyActive)
            : base(x => !onlyActive || (onlyActive && x.ShowOnShop))
        {
            AddInclude(x => x.CatalogPictures);
        }

        public CatalogFilterSpecification(string slug)
            : base(x => x.Slug == slug)
        {
            AddInclude(x => x.CatalogCategories);
            AddInclude($"{nameof(CatalogItem.CatalogCategories)}.{nameof(CatalogCategory.Category)}");
            AddInclude(x => x.CatalogPictures);
            AddInclude(x => x.CatalogAttributes);
            AddInclude(x => x.CatalogType);
            AddInclude(x => x.CatalogIllustration);
            AddInclude($"{nameof(CatalogItem.CatalogIllustration)}.{nameof(CatalogIllustration.IllustrationType)}");
            AddInclude(x => x.CatalogType);
            AddInclude($"{nameof(CatalogItem.CatalogType)}.{nameof(CatalogType.PictureTextHelpers)}");
            AddInclude(x => x.CatalogReferences);
            AddInclude($"{nameof(CatalogItem.CatalogReferences)}.{nameof(CatalogReference.ReferenceCatalogItem)}");
        }
    }

    public class CatalogSearchSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogSearchSpecification(string searchFor, bool showOnlyAvailabe)
            : base(x => x.ShowOnShop &&
                (!showOnlyAvailabe || (showOnlyAvailabe && !x.IsUnavailable)) &&
                (x.CatalogType.Name.Contains(searchFor) ||
                x.CatalogIllustration.Name.Contains(searchFor) ||
                x.CatalogIllustration.IllustrationType.Name.Contains(searchFor) ||
                x.Name.Contains(searchFor) ||
                x.Description.Contains(searchFor)))
        {
            AddInclude(x => x.CatalogType);
            AddInclude(x => x.CatalogIllustration);
            AddInclude($"{nameof(CatalogItem.CatalogIllustration)}.{nameof(CatalogIllustration.IllustrationType)}");
        }
    }

    public class CatalogAttrFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogAttrFilterSpecification(int catalogId) : base(i => i.Id == catalogId)
        {
            AddInclude(x => x.CatalogAttributes);
        }
    }

    public class CatalogTypeFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogTypeFilterSpecification(int catalogId) : base(i => i.Id == catalogId)
        {
            AddInclude(x => x.CatalogType);
            AddInclude(x => x.CatalogAttributes);
        }
    }

    public class CatalogSkuSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogSkuSpecification(string sku) : base(i => i.Sku == sku)
        {

        }
    }

    public class CatalogTagSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogTagSpecification(string tagName, TagType? tagType, bool showOnlyAvailable) : 
            base(x => x.ShowOnShop &&
            (!showOnlyAvailable || (showOnlyAvailable && !x.IsUnavailable)) &&
            (
            (tagType.HasValue && tagType.Value == TagType.CATALOG_TYPE && x.CatalogType.Slug == tagName) || 
            (tagType.HasValue && tagType.Value == TagType.ILLUSTRATION && x.CatalogIllustration.Name == tagName) ||
            (tagType.HasValue && tagType.Value == TagType.ILLUSTRATION_TYPE && x.CatalogIllustration.IllustrationType.Name == tagName) || 
            (!tagType.HasValue && (x.CatalogType.Slug == tagName || x.CatalogIllustration.Name == tagName || x.CatalogIllustration.IllustrationType.Name == tagName))))
        {
            AddInclude(x => x.CatalogType);
            AddInclude(x => x.CatalogIllustration);
            AddInclude($"{nameof(CatalogItem.CatalogIllustration)}.{nameof(CatalogIllustration.IllustrationType)}");
        }
    }
}
