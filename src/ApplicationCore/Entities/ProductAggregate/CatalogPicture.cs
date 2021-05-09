using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CatalogPicture : BaseEntity
    {
        public string PictureUri { get; private set; }
        public string PictureHighUri { get; private set; }
        public string PictureLowUri { get; private set; }
        public bool IsMain { get; private set; }
        public bool IsActive { get; private set; }
        public int Order { get; private set; }
        public int CatalogItemId { get; private set; }
        public CatalogItem CatalogItem { get; private set; }

        public CatalogPicture(bool isActive, bool isMain, string pictureUri, int order, string pictureHighUri = null)
        {
            IsActive = isActive;
            IsMain = isMain;
            PictureUri = pictureUri;
            Order = 0;
            if (!string.IsNullOrEmpty(pictureHighUri))
                PictureHighUri = pictureHighUri;
        }

        public void UpdatePictureInfo(bool isActive, int order, string pictureUri, string pictureHighUri)
        {
            IsActive = isActive;
            Order = order;
            PictureUri = pictureUri;
            PictureHighUri = pictureHighUri;
        }

        public void UpdatePictureUri(string pictureUri)
        {
            PictureUri = pictureUri;
        }
    }
}
