using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CatalogTypeSpecification : Specification<CatalogType>
    {
        public CatalogTypeSpecification(int categoryId)
        {
            Query
                .Include(x => x.CatalogItems)
                .Where(x => x.Categories.Any(c => c.CategoryId == categoryId));
        }

        public CatalogTypeSpecification(bool includeHelpers)
        {
            if(includeHelpers)
                Query.Include(x => x.PictureTextHelpers);
        }
        
        public CatalogTypeSpecification(int? pageIndex, int? pageSize)        
        {
            Query
                .Include(t => t.Categories)
                .ThenInclude(c => c.Category);
            
            if(pageIndex.HasValue && pageSize.HasValue)
            {
                Query.Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }
        }
    }
}
