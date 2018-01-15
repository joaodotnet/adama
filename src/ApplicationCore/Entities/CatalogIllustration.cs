using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class CatalogIllustration : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public int IllustrationTypeId { get; set; }
        public IllustrationType IllustrationType { get; set; }
        public byte[] Image { get; set; }
    }
}
