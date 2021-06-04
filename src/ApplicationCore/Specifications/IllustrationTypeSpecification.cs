using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications
{
    public class IllustrationTypeSpecification : Specification<IllustrationType>
    {
        public IllustrationTypeSpecification(string code, int? notId = null)
        {
            Query.Where(x => x.Code.ToUpper() == code.ToUpper());
            if(notId.HasValue)
                Query.Where(x => x.Id != notId.Value);
        }
    }
}
