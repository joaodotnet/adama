using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamaWeb.ViewModels;

namespace DamaWeb.ViewModels
{
    public class CategoryViewModel
    {
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();
        //public List<(string,string)> CatalogTypes { get; set; }
        public string CategoryUrlName { get; set; }
        public string CategoryName { get; set; }
        public string MetaDescription { get; set; }
        public string Title { get; set; }
    }
}
