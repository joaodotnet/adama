using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ShopConfigDetail : BaseEntity
    {
        public string PictureUri { get; set; }
        public string PictureWebpUri { get; set; }
        public string PictureMobileUri { get; set; }
        public string HeadingText { get; set; }
        public string ContentText { get; set; }
        public string LinkButtonUri { get; set; }
        public string LinkButtonText { get; set; }
        public bool IsActive { get; set; } = true;
        public int ShopConfigId { get; set; }
        public ShopConfig ShopConfig { get; set; }
    }
}
