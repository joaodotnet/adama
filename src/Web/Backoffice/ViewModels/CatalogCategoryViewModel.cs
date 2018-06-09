using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class CatalogCategoryViewModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Label { get; set; }
        public bool Selected { get; set; }
        public List<CatalogCategoryViewModel> Childs { get; set; } = new List<CatalogCategoryViewModel>();
    }
}
