using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.ViewModels
{
    public class CategoryViewModel
    {
        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();
        public List<(string,string)> CatalogTypes { get; set; }
    }
}
