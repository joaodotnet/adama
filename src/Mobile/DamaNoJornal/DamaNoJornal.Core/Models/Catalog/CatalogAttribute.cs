namespace DamaNoJornal.Core.Models.Catalog
{
    public class CatalogAttribute
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public CatalogAttributeType Type { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }        

        public string GetTypeDescription()
        {
            switch (Type)
            {
                case CatalogAttributeType.SIZE:
                    return "Tamanho";
                case CatalogAttributeType.BOOK_FORMAT:
                    return "Formato";
                case CatalogAttributeType.Color:
                    return "Cor";
                default:
                    return "";
            }
        }
    }
}