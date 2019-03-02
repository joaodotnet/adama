using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string MetaDescription { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public Category Parent { get; set; }
        //public virtual ICollection<CatalogType> CatalogTypes { get; set; }
        public ICollection<CatalogTypeCategory> CatalogTypes { get; set; }
        public ICollection<CatalogCategory> CatalogCategories { get; set; }
    }
}
