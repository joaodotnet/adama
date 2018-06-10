using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DamaWeb.ViewModels
{
    public class CatalogIndexViewModel
    {
        public IEnumerable<CatalogItemViewModel> CatalogItems { get; set; }
        public IEnumerable<CatalogItemViewModel> NewCatalogItems { get; set; }
        public IEnumerable<CatalogItemViewModel> FeaturedCatalogItems { get; set; }
        public IEnumerable<SelectListItem> Illustrations { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public int? IllustrationFilterApplied { get; set; }
        public int? TypesFilterApplied { get; set; }
        public PaginationInfoViewModel PaginationInfo { get; set; }
        public IEnumerable<CatalogTypeViewModel> CatalogTypes { get; set; }
        public string SearchFor { get; set; }

        //Dama Stuff
        public List<MainBannerViewModel> Banners { get; set; }
    }
}
