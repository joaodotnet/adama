using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string Position { get; set; }
        public int? ParentId { get; set; }
        public Category Parent { get; set; }
        public virtual ICollection<CatalogType> CatalogTypes { get; set; }
    }
}
