using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogIllustrationSpecification : Specification<CatalogIllustration>, ISingleResultSpecification
    {
        public CatalogIllustrationSpecification()
        {
            Query.Include(x => x.IllustrationType)
                .OrderBy(x => x.Code);
        }
        public CatalogIllustrationSpecification(string code, int? notId = null)
        {
            Query.Where(x => x.Code.ToUpper() == code.ToUpper());
            if(notId.HasValue)
                Query.Where(x => x.Id != notId.Value);
        }

        public CatalogIllustrationSpecification(int id)
        {
            Query
                .Include(x => x.IllustrationType)
                .Where(x => x.Id == id);
        }

        public CatalogIllustrationSpecification(bool showInMenu)
        {
            Query.Where(x => x.InMenu == showInMenu);
        }
    }
}
