using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogFilterSpecification : Specification<CatalogItem>
    {
        public CatalogFilterSpecification(int? IllustrationId, int? typeId, int? categoryId, bool? canCustomize = null, bool? showOnlyAvailable = null)
        {
            Query
                .Include(x => x.CatalogType)
                .Include(x => x.Categories)
                .Where(i => i.ShowOnShop &&
                    (!IllustrationId.HasValue || i.CatalogIllustrationId == IllustrationId) &&
                    (!typeId.HasValue || i.CatalogTypeId == typeId) &&
                    (!categoryId.HasValue || i.Categories.Any(x => x.CategoryId == categoryId)) &&
                    (!canCustomize.HasValue || i.CanCustomize == canCustomize.Value) &&
                    (!showOnlyAvailable.HasValue || (showOnlyAvailable.HasValue && !showOnlyAvailable.Value) || (showOnlyAvailable.HasValue && showOnlyAvailable.Value && !i.IsUnavailable)));
        }

        public CatalogFilterSpecification(bool onlyActive)
        {
            Query
                .Include(x => x.Pictures)
                .Where(x => !onlyActive || (onlyActive && x.ShowOnShop));
        }
    }
}
