using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class OrderItemViewModel
    {
        public string ProductSku { get; set; }
        public string ProductName { get; set; }
        public string PictureUri { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
        public List<OrderItemAttributeViewModel> Attributes { get; set; }
    }

    public class OrderItemAttributeViewModel
    {
        public CatalogAttributeType AttributeType { get; set; }
        public string AttributeName { get; set; }
    }
}
