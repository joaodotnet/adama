using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWeb.ViewModels;

namespace SalesWeb.ViewModels
{
    public class CategoryViewModel
    {
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();
        //public List<(string,string)> CatalogTypes { get; set; }
        public string CategoryUrlName { get; set; }
        public string CategoryName { get; set; }

    }
}
