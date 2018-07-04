using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Models.Basket
{
    public class BasketItem : BindableObject
    {
        private int _quantity;

        public int Id { get; set; }
       
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal OldUnitPrice { get; set; }
        public List<BasketItemAttribute> Attributes { get; set; } = new List<BasketItemAttribute>();

        public bool HasNewPrice
        {
            get
            {
                return OldUnitPrice != 0.0m;
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
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