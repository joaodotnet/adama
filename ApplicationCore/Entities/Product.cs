using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Size> Sizes { get; set; }
        public List<Option> Options { get; set; }
        public Illustration Illustation { get; set; }
        public Category Category { get; set; }
        public ProductType ProductType { get; set; }
    }
}
