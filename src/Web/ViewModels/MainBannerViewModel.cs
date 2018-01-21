using System.Collections.Generic;

namespace DamaShopWeb.Web.ViewModels
{
    public class MainBannerViewModel
    {
        public bool? IsActive { get; set; }
        public List<MainBannerDetailsViewModel> Details { get; set; }
    }

    public class MainBannerDetailsViewModel
    {
        public string PictureUri { get; set; }
        public string HeadingText { get; set; }
        public string ContentText { get; set; }
        public string LinkButtonUri { get; set; }
        public string LinkButtonText { get; set; }
        public bool? IsActive { get; set; }
    }
}