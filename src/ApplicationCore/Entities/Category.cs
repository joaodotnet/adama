using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public int Order { get; private set; }
        public string MetaDescription { get; private set; }
        public string Title { get; private set; }
        public string H1Text { get; private set; }
        public string Description { get; private set; }
        public int? ParentId { get; private set; }
        public Category Parent { get; private set; }
        //public virtual ICollection<CatalogType> CatalogTypes { get; set; }
        private readonly List<CatalogTypeCategory> _catalogTypes = new ();
        public IReadOnlyCollection<CatalogTypeCategory> CatalogTypes => _catalogTypes.AsReadOnly();
        private readonly List<CatalogCategory> _catalogCategories = new();
        public IReadOnlyCollection<CatalogCategory> CatalogCategories => _catalogCategories.AsReadOnly();
    }
}
