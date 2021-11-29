using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class IllustrationTypeSpecification : Specification<IllustrationType>
    {
        public IllustrationTypeSpecification(IllustrationTypeFilter filter)
        {
            if(!string.IsNullOrEmpty(filter.Code))
                Query.Where(x => x.Code.ToUpper() == filter.Code.ToUpper());
            
            if (filter.NotId.HasValue)
                Query.Where(x => x.Id != filter.NotId.Value);
            
            if (filter.PageIndex.HasValue && filter.PageSize.HasValue)
            {
                Query.Skip((filter.PageIndex.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value);
            }
        }
    }
    public class IllustrationTypeFilter
    {
        public string Code { get; set; }
        public int? NotId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
