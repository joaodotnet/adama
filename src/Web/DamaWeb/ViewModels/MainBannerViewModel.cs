namespace DamaWeb.ViewModels
{
    public class MainBannerViewModel
    {
        public string PictureUri { get; set; }
        public string HeadingText { get; set; }
        public string ContentText { get; set; }
        public string LinkButtonUri { get; set; }
        public string LinkButtonText { get; set; }
        public bool? IsActive { get; set; }
        public string LinkClass { get {
                return (!string.IsNullOrEmpty(LinkButtonUri) ?
                        "bannerLink" : "");
            } }
    }    
}