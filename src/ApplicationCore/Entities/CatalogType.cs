using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class CatalogType : BaseEntity
    {
        public string Code { get; set; } //To Remove       
        public string Description { get; set; }
        public string PictureUri { get; set; }
        //public int CategoryId { get; set; }
        //public Category Category { get; set; }

        public ICollection<CatalogTypeCategory> Categories { get; set; }
        public ICollection<CatalogItem> CatalogItems { get; set; }
    }
}
