using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeSpecification : Specification<CatalogType>
    {
        public class CataloTypeFilter
        {
            public int? CategoryId { get; set; }
            public bool? IncludeHelpers { get; set; }
            public int? PageIndex { get; set; }
            public int? PageSize { get; set; }
            public string Code { get; set; }
            public string Slug { get; set; }
        }
        public CatalogTypeSpecification(CataloTypeFilter filter)
        {
            if(filter.CategoryId.HasValue)
            {
                Query
                .Include(x => x.CatalogItems)
                .Where(x => x.Categories.Any(c => c.CategoryId == filter.CategoryId));
            }

            if(filter.IncludeHelpers == true)
            {
                 Query.Include(x => x.PictureTextHelpers);
            }

            if(filter.PageIndex.HasValue && filter.PageSize.HasValue)
            {
                Query.Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value);

            }

            if(!string.IsNullOrEmpty(filter.Code))
            {
                Query
                    .Where(x => x.Code.ToUpper() == filter.Code);
            }

            if(!string.IsNullOrEmpty(filter.Slug))
            {
                Query
                    .Where(x => x.Slug.ToUpper() == filter.Slug);
            }

            //Todo: Make it optional
            Query
                .Include(t => t.Categories)
                .ThenInclude(c => c.Category);


        }   
    }
}
