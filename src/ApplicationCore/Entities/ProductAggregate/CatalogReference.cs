using System;

namespace ApplicationCore.Entities
{
    public class CatalogReference : BaseEntity
    {
        public CatalogReference(int catalogItemId, string labelDescription, int referenceCatalogItemId)
        {
            CatalogItemId = catalogItemId;
            LabelDescription = labelDescription;
            ReferenceCatalogItemId = referenceCatalogItemId;
        }
        public CatalogReference(string labelDescription, int referenceCatalogItemId)
        {
            LabelDescription = labelDescription;
            ReferenceCatalogItemId = referenceCatalogItemId;
        }

        public int CatalogItemId { get; private set; }
        public CatalogItem CatalogItem { get; private set; }
        public int ReferenceCatalogItemId { get; private set; }
        public CatalogItem ReferenceCatalogItem { get; private set; }
        public string LabelDescription { get; private set; }

        public void UpdateLabel(string labelDescription)
        {
            LabelDescription = labelDescription;
        }
    }
}
