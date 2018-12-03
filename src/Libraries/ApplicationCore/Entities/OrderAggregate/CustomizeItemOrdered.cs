namespace ApplicationCore.Entities.OrderAggregate
{
    public class CustomizeItemOrdered
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Colors { get; set; }

        //Check
        public int? CatalogTypeId { get; set; }
        public string ProductName { get; set; }
        public string PictureUri { get; set; }
    }
}