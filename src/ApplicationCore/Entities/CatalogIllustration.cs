using System;
using System.Collections.Generic;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Entities
{
    public class CatalogIllustration : BaseEntity, IAggregateRoot
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string PictureUri { get; private set; }
        public int IllustrationTypeId { get; private set; }
        public IllustrationType IllustrationType { get; private set; }
        public byte[] Image { get; private set; }

        public bool InMenu { get; private set; }

        public CatalogIllustration(string code, string name, int illustrationTypeId)
        {
            Code = code;
            Name = name;
            IllustrationTypeId = illustrationTypeId;
        }

        public void UpdateImage(byte[] image)
        {
            Image = image;
        }

        public void UpdateData(string code, string name, int illustrationTypeId, bool inMenu)
        {
            Code = code;
            Name = name;
            IllustrationTypeId = illustrationTypeId;
            InMenu = inMenu;
        }

        public void UpdateInMenuFlag(bool value)
        {
            InMenu = value;
        }
    }
}
