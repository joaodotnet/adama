using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Entities
{
    public class Basket : BaseEntity
    {
        public string BuyerId { get; set; }
        private readonly List<BasketItem> _items = new List<BasketItem>();
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

        public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1, int? option1 = null, int? option2 = null, int? option3 = null, string customizeName = null, string customizeSide = null, bool addToExistingItem = false)
        {
            if(!addToExistingItem)
            {
                _items.Add(new BasketItem()
                {
                    CatalogItemId = catalogItemId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    CatalogAttribute1 = option1,
                    CatalogAttribute2 = option2,
                    CatalogAttribute3 = option3,
                    CustomizeName = customizeName,
                    CustomizeSide = customizeSide
                });
            }
            else
            {
                var existingItem = Items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
                existingItem.Quantity += quantity;                
            }

        }

        public void RemoveItem(int index)
        {
            _items.RemoveAt(index);
        }

        public void RemoveAllItems()
        {
            _items.RemoveAll(_ => true);
        }
    }
}
