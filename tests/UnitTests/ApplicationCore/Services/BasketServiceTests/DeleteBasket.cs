using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ApplicationCore.Services.BasketServiceTests
{
    public class DeleteBasket
    {
        private Mock<IAsyncRepository<Basket>> _mockBasketRepo;

        public DeleteBasket()
        {
            _mockBasketRepo = new Mock<IAsyncRepository<Basket>>();
        }

        [Fact]
        public async Task Should_InvokeBasketRepositoryDeleteAsync_Once()
        {
            var basket = new Basket();
            basket.AddItem(1, It.IsAny<decimal>(), It.IsAny<int>());
            basket.AddItem(2, It.IsAny<decimal>(), It.IsAny<int>());
            _mockBasketRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(basket);
            var basketService = new BasketService(_mockBasketRepo.Object, Mock.Of<IAsyncRepository<CatalogItem>>(), null);

            await basketService.DeleteBasketAsync(It.IsAny<int>());

            _mockBasketRepo.Verify(x => x.DeleteAsync(It.IsAny<Basket>()), Times.Once);
        }
    }
}
