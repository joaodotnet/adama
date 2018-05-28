namespace DamaNoJornal.Core.Models.Catalog
{
    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}