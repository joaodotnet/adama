using ApplicationCore.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IBasketService
    {
        Task<int> GetBasketItemCountAsync(string userName);
        Task TransferBasketAsync(string anonymousId, string userName);
        Task AddItemToBasket(int basketId, int catalogItemId, decimal price, int quantity, List<int> attrIds = null);
        Task SetQuantities(int basketId, Dictionary<string, int> quantities);
        Task DeleteItem(int basketId, int itemIndex);
        Task DeleteBasketAsync(int basketId);
        Task<DeliveryTimeDTO> CalculateDeliveryTime(int basketId);
    }
}
