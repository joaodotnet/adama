using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogItemSpecification : Specification<CatalogItem>, ISingleResultSpecification<CatalogItem>
    {
        public CatalogItemSpecification(CatalogItemFilter filter)
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

            if(filter.AddPictures)
            {
                Query.Include(p => p.Pictures);
            }

            if (!string.IsNullOrEmpty(filter.Slug))
            {
                Query
                    .Where(x => x.Slug.ToUpper() == filter.Slug);
            }

            if (filter.NotProductId.HasValue)
                Query.Where(x => x.Id != filter.NotProductId);

            if(filter.ProductId.HasValue)
                Query.Where(x => x.Id == filter.ProductId);

            if (filter.OrderByIdDesc)
                Query.OrderByDescending(x => x.Id);

            if (filter.PageIndex.HasValue && filter.PageSize.HasValue)
            {
                Query.Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value);
            }
        }
    }

    public class CatalogItemFilter
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public bool AddCategories { get; set; }
        public bool AddAttributes { get; set; }
        public bool AddIllustrations { get; set; }
        public bool AddPictures { get; set; }
        public string Slug { get; set; }
        public int? NotProductId { get; set; }
        public int? ProductId { get; set; }
        public bool OrderByIdDesc{ get; set; }
    }
}
