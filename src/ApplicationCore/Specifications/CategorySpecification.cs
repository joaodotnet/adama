using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CategorySpecification : Specification<Category>, ISingleResultSpecification
    {
        public CategorySpecification()
        {
            Query
                .Include(x => x.CatalogCategories)
                .Where(x => !x.ParentId.HasValue && x.CatalogCategories.Any(c => c.CatalogItem.CanCustomize));
        }

        public CategorySpecification(bool forMenuList)
        {
            Query
                .Include(x => x.Parent)
                .Include(x => x.CatalogCategories)
                .Include($"{nameof(Category.CatalogCategories)}.{nameof(CatalogCategory.CatalogItem)}")
                .Include(x => x.CatalogTypes)
                .Include($"{nameof(Category.CatalogTypes)}.{nameof(CatalogTypeCategory.CatalogType)}")
                .Where(x => x.CatalogCategories.Any())
                .OrderBy(x => x.Order);
        }

        public CategorySpecification(CategoryFilter filter)
        {
            if(!string.IsNullOrWhiteSpace(filter.Name))
                Query.Where(x => x.Name.ToUpper() == filter.Name.ToUpper());
            if(!string.IsNullOrWhiteSpace(filter.Slug))
                Query.Where(x => x.Slug == filter.Slug);
            if(filter.Id.HasValue)
                Query.Where(x => x.Id == filter.Id.Value);
            if(filter.NotTheSameId)
                Query.Where(x => x.Id != filter.Id.Value);
            if(filter.IncludeParent)
                Query.Include(x => x.Parent);
            if(filter.IncludeCatalogTypes)
                Query.Include(x => x.CatalogTypes);
        }
    }

    public class CategoryFilter
    {
        public int? Id {get; set;}

        public string Name { get; set; }
        public string Slug { get; set; }  
        public bool IncludeParent { get; set; }        
        public bool IncludeCatalogTypes { get; set; }
        public bool NotTheSameId { get; set; }
    }
}
