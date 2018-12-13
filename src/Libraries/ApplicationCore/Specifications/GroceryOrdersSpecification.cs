using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;

namespace ApplicationCore.Specifications
{
    public class GroceryOrdersSpecification : BaseSpecification<Order>
    {       
        public GroceryOrdersSpecification(int? id = null)
            :base(o => o.SalesInvoiceId.HasValue && ((id.HasValue && o.Id == id) || !id.HasValue ))
        {
            AddInclude(o => o.OrderItems);            
        }        
    }
}
