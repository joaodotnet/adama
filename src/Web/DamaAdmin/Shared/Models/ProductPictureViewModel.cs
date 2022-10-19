namespace DamaAdmin.Shared.Models
{
    public class ProductPictureViewModel
    {
        public int Id { get; set; }
        public string PictureUri { get; set; }
        public bool IsActive { get; set; }
        public bool IsMain { get; set; }
        public int Order { get; set; }
        public int CatalogItemId { get; set; }
        public bool ToRemove { get; set; }
        public string PictureHighUri { get; set; }
    }
}
