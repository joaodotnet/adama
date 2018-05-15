using System;

namespace Dama.API.ViewModels
{
    public class BasketItemViewModel
    {
        public int Quantity { get; set; }

        public int Id { get; set; }
       
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal OldUnitPrice { get; set; }

        public bool HasNewPrice
        {
            get
            {
                return OldUnitPrice != 0.0m;
            }
        }       

        public string PictureUrl { get; set; }

        public decimal Total { get { return Quantity * UnitPrice; } }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", ProductId, Quantity);
        }
    }
}