using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaSalesApp.Models
{
    public class Product
    {
        public int Id { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? Personalized { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
        //public ICollection<ProductAttribute> ProductAttributes { get; set; }
    }
}
