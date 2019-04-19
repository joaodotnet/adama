using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CatalogPicture : BaseEntity
    {
        public string PictureUri { get; set; }
        public string PictureHighUri { get; set; }
        public string PictureLowUri { get; set; }
        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }
    }
}
