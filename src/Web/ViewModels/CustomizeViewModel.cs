using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class CustomizeViewModel
    {
        public int CategorySelected { get; set; }
        public List<(int,string)> Categories { get; set; }
        //public List<CatalogIndexViewModel> CatalogItems { get; set; }
    }
}
