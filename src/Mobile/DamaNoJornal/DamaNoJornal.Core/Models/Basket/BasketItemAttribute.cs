using DamaNoJornal.Core.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DamaNoJornal.Core.Models.Basket
{
    public class BasketItemAttribute : BindableObject
    {
        public int Id { get; set; }
        public AttributeType Type { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string DisplayName
        {
            get
            {
                return $"{AttributeTypeHelper.GetTypeDescription(Type)} {Name}";
            }
        }

        public override string ToString()
        {
            return $"Attribute Id: {Id}, Name: {Name}, Type: {Type}";
        }
    }
}
