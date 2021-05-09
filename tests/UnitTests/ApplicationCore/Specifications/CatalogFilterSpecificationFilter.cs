using ApplicationCore.Specifications;
using ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests
{
    public class CatalogFilterSpecificationFilter
    {
        [Theory]
        [InlineData(null, null, 5)]
        [InlineData(1, null, 3)]
        [InlineData(2, null, 2)]
        [InlineData(null, 1, 2)]
        [InlineData(null, 3, 1)]
        [InlineData(1, 3, 1)]
        [InlineData(2, 3, 0)]
        public void MatchesExpectedNumberOfItems(int? illustrationId, int? typeId, int expectedCount)
        {
            var spec = new CatalogFilterSpecification(illustrationId, typeId, null);

            var result = GetTestItemCollection()
                .AsQueryable()
                .Where(spec.Criteria);

            Assert.Equal(expectedCount, result.Count());
        }

        public List<CatalogItem> GetTestItemCollection()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem(1,1,true) { Id = 1 },
                new CatalogItem(1,2,true) { Id = 2 },
                new CatalogItem(1,3,true) { Id = 3 },
                new CatalogItem(2,1,true) { Id = 4 },
                new CatalogItem(2,2,true) { Id = 5 },
            };
        }
    }
}
