using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ShopConfig : BaseEntity
    {
        public ShopConfigType Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool? IsActive { get; set; }

        public List<ShopConfigDetail> Details { get; set; }
    }

    public enum ShopConfigType
    {        
        NEWS_BANNER
    }
}
