using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogSearchSpecification : Specification<CatalogItem>
    {
        public CatalogSearchSpecification(string searchFor, bool showOnlyAvailabe)
        {
            Query
                .Include(x => x.CatalogType)
                .Include(x => x.CatalogIllustration)
                .Include($"{nameof(CatalogItem.CatalogIllustration)}.{nameof(CatalogIllustration.IllustrationType)}")
                .Where(x => x.ShowOnShop &&
                    (!showOnlyAvailabe || (showOnlyAvailabe && !x.IsUnavailable)) &&
                    (x.CatalogType.Name.Contains(searchFor) ||
                    x.CatalogIllustration.Name.Contains(searchFor) ||
                    x.CatalogIllustration.IllustrationType.Name.Contains(searchFor) ||
                    x.Name.Contains(searchFor) ||
                    x.Description.Contains(searchFor)));
        }
    }
}
