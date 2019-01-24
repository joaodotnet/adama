namespace ApplicationCore.Entities.OrderAggregate
{
    public class CustomizeItemOrdered
    {
        public CustomizeItemOrdered(int catalogTypeId, string description, string name, string colors, string productName, string pictureUri)
        {
            CatalogTypeId = catalogTypeId;
            Description = description;
            Name = name;
            Colors = colors;
            ProductName = productName;
            PictureUri = pictureUri;
        }
        public CustomizeItemOrdered()
        {

        }

        public string Description { get; private set; }
        public string Name { get; private set; }
        public string Colors { get; private set; }

        //Check
        public int? CatalogTypeId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUri { get; private set; }
    }
}