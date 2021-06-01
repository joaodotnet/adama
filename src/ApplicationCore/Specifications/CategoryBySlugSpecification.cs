using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class CategoryBySlugSpecification : Specification<Category>, ISingleResultSpecification
    {
        public CategoryBySlugSpecification(string slug)
        {
            Query.Where(x => x.Slug == slug);
        }
    }
}
