namespace ApplicationCore.Entities.BasketAggregate
{
    public class BasketDetailItem : BaseEntity
    {
        public int CatalogAttributeId { get; set; }
        public CatalogAttribute CatalogAttribute { get; set; }
        public int BasketItemId { get; set; }
        public BasketItem BasketItem { get; set; }

    }
}