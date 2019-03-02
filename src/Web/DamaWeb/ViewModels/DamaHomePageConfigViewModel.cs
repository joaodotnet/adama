using System.Collections.Generic;

namespace DamaWeb.ViewModels
{
    public class DamaHomePageConfigViewModel
    {
        public string MetaDescription { get; set; }
        public List<MainBannerViewModel> Banners { get; set; }
        public string Title { get; set; }
    }
}