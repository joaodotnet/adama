using ApplicationCore.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ApplicationCore.Services.BasketServiceTests
{
    public class TransferBasket
    {
        [Fact]
        public async Task ThrowsGivenNullAnonymousId()
        {
            var basketService = new BasketService(null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await basketService.TransferBasketAsync(null, "steve", false));
        }

        [Fact]
        public async Task ThrowsGivenNullUserId()
        {
            var basketService = new BasketService(null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await basketService.TransferBasketAsync("abcdefg", null, false));
        }
    }
}
