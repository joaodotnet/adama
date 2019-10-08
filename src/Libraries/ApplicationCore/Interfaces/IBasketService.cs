using ApplicationCore.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IBasketService
    {
        Task<int> GetBasketItemCountAsync(string userName);
        Task TransferBasketAsync(string anonymousId, string userName, bool isGuest);
        Task AddItemToBasket(int basketId, int catalogItemId, decimal price, int quantity, int? option1 = null, int? option2 = null, int? option3 = null, string customizeName = null, string customizeSide = null, bool addToExistingItem = false);
        Task SetQuantities(int basketId, Dictionary<string, int> quantities);
        Task DeleteItem(int basketId, int itemIndex);
        Task DeleteBasketAsync(int basketId);
        Task<DeliveryTimeDTO> CalculateDeliveryTime(int basketId);
        Task<(int?,int?,int?)> GetFirstOptionFromAttributeAsync(int catalogItemId);
        Task AddCustomizeItemToBasket(int basketId, int catalogTypeId, string description, string textOrName, string colors, int quantity);
        Task AddObservationAsync(int basketId, string message);
        Task RemoveObservationsAsync(int basketId);
    }
}
