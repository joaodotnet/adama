namespace DamaNoJornal.Core.Models.Catalog
{
    public class CatalogType
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}