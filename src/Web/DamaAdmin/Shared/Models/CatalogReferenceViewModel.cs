namespace DamaAdmin.Shared.Models
{
    public class CatalogReferenceViewModel
    {
        public int Id { get; set; }
        public string LabelDescription { get; set; }
        public int CatalogItemId { get; set; }
        public string CatalogItemName { get; set; }
        public int ReferenceCatalogItemId { get; set; }
        public string ReferenceCatalogItemName { get; set;}
    }
}
