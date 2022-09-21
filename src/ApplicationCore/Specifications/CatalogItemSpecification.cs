using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogItemSpecification : Specification<CatalogItem>, ISingleResultSpecification
    {
        public CatalogItemSpecification(CatalogItemFiler filter)
        {
            if(filter.AddIllustrations)
            {
                Query.Include(p => p.CatalogIllustration)
                    .ThenInclude(i => i.IllustrationType)
                        .Include(p => p.CatalogType);
            }
            if(filter.AddCategories)
            {
                Query.Include(p => p.Categories)
                    .ThenInclude( cc => cc.Category);
            }

            if(filter.AddAttributes)
            {
                Query.Include(p => p.Attributes);
            }

            if (filter.PageIndex.HasValue && filter.PageSize.HasValue)
            {
                Query.Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value);
            }
        }
    }

    public class CatalogItemFiler
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public bool AddCategories { get; set; }
        public bool AddAttributes { get; set; }
        public bool AddIllustrations { get; set; }
    }
}
