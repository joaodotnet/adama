using System.Collections.Generic;

namespace ApplicationCore.Entities
{
    public class BasketItem : BaseEntity
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int CatalogItemId { get; set; }
        public int? CatalogAttribute1 { get; set; }        
        public int? CatalogAttribute2 { get; set; }
        public int? CatalogAttribute3 { get; set; }
        public string CustomizeName { get; set; }
        public string CustomizeSide { get; set; }
        public string CustomizeDescription { get; set; }
        public string CustomizeColors { get; set; }
        public int? CatalogTypeId { get; set; }

        public int BasketId { get; set; }
        public Basket Basket { get; set; }
                                           //public CatalogAttribute CatalogAttribute1 { get; set; }
                                           //public CatalogAttribute CatalogAttribute2 { get; set; }
                                           //public CatalogAttribute CatalogAttribute3 { get; set; }

        //public IList<CatalogAttribute> CatalogAttributes
        //{
        //    get
        //    {
        //        var catalogAttributes = new List<CatalogAttribute>();
        //        if (CatalogAttribute1 != null)
        //            catalogAttributes.Add(CatalogAttribute1);
        //        if (CatalogAttribute2 != null)
        //            catalogAttributes.Add(CatalogAttribute2);
        //        if (CatalogAttribute3 != null)
        //            catalogAttributes.Add(CatalogAttribute3);

        //        return catalogAttributes;
        //    }
        //}


        //public List<BasketDetailItem> Details { get; set; }
    }
}
