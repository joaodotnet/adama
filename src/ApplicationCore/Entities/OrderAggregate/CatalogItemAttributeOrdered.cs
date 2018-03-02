namespace ApplicationCore.Entities.OrderAggregate
{
    /// <summary>
    /// Represents the item attribute that was ordered. If catalog item details change, details of
    /// the item that was part of a completed order should not change.
    /// </summary>
    public class CatalogItemAttributeOrdered 
    {
        //public CatalogItemAttributeOrdered(int catalogItemAttributeId, CatalogAttributeType type, string attributeCode, string attributeName)
        //{
        //    CatalogItemAttributeId = catalogItemAttributeId;
        //    AttributeType = type;
        //    AttributeCode = attributeCode;
        //    AttributeName = attributeName;

        //}
        //private CatalogItemAttributeOrdered()
        //{
        //    // required by EF
        //}
        //public int CatalogItemAttributeId { get; private set; }
        //public CatalogAttributeType AttributeType { get; private set; }
        //public string AttributeCode { get; private set; }
        //public string AttributeName { get; private set; }
        //public CatalogItemOrdered CatalogItemOrdered { get; set; }
    }
}
