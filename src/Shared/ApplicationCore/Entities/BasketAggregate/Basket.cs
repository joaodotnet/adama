using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Entities.BasketAggregate
{
    public class Basket : BaseEntity, IAggregateRoot
    {
        public string BuyerId { get; set; }
        private readonly List<BasketItem> _items = new List<BasketItem>();
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Observations { get; set; }
        public bool IsGuest { get; set; }

        public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1, int? option1 = null, int? option2 = null, int? option3 = null, string customizeName = null, string customizeSide = null, bool addToExistingItem = false)
        {
            if(!addToExistingItem ||(addToExistingItem && !Items.Any(i => i.CatalogItemId == catalogItemId)))
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
                    CustomizeSide = customizeSide,
                    CreatedDate = DateTime.Now
                });
            }
            else
            {
                var existingItem = Items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
                existingItem.Quantity += quantity;
                existingItem.UpdatedDate = DateTime.Now;
            }

        }

        public void AddCustomizeItem(int catalogTypeId, string description, string textOrName, string colors, int quantity = 1)
        {
            _items.Add(new BasketItem()
            {
                CatalogTypeId = catalogTypeId,
                CustomizeDescription = description,
                CustomizeName = textOrName,
                CustomizeColors = colors,
                Quantity = quantity,
                CreatedDate = DateTime.Now
            });
        }

        public void RemoveItem(int index)
        {
            _items.RemoveAt(index);
        }

        public void RemoveAllItems()
        {
            _items.RemoveAll(_ => true);
        }

        public void RemoveEmptyItems()
        {
            _items.RemoveAll(i => i.Quantity == 0);
        }
    }
}
