using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class FileDetail : BaseEntity
    {
        public string PictureUri { get; set; }
        public string Location { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public bool? IsActive { get; set; }
        public int? Order { get; set; }

        public int? CatalogTypeId { get; set; }
        public CatalogType CatalogType { get; set; }
    }
}
