﻿
namespace ApplicationCore.Entities
{
    public class CatalogAttribute : BaseEntity
    {
        public AttributeType Type { get; private set; }
        public string Name { get; private set; }
        public int CatalogItemId { get; private set; }
        public CatalogItem CatalogItem { get; private set; }
        public int Stock { get; private set; }

        public void UpdateStock(int newStock)
        {
            Stock = newStock;
        }
    }
}