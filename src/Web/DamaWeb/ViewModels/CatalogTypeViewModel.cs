using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb.ViewModels
{
    public class CatalogTypeViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public string CatNameUri { get; set; }
        public string TypeNameUri { get; set; }
    }
}
