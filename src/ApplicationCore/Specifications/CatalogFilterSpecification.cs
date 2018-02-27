using ApplicationCore.Entities;

namespace ApplicationCore.Specifications
{

    public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogFilterSpecification(int? IllustrationId, int? typeId)
            : base(i => i.ShowOnShop && (!IllustrationId.HasValue || i.CatalogIllustrationId == IllustrationId) &&
                (!typeId.HasValue || i.CatalogTypeId == typeId))
        {
            AddInclude(x => x.CatalogType);
        }
    }

    public class CatalogAttrFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogAttrFilterSpecification(int catalogId): base(i => i.Id == catalogId)
        {
            AddInclude(x => x.CatalogAttributes);
        }
    }
}
