using ApplicationCore.Entities;

namespace ApplicationCore.Specifications
{

    public class CatalogFilterSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogFilterSpecification(int? brandId, int? typeId)
            : base(i => i.ShowOnShop && (!brandId.HasValue || i.CatalogType.CategoryId == brandId) &&
                (!typeId.HasValue || i.CatalogTypeId == typeId))
        {
            AddInclude(x => x.CatalogType);
        }
    }
}
