using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.ViewModels;
using DamaWeb.Interfaces;
using ApplicationCore.Interfaces;
using ApplicationCore;

namespace DamaWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IShopService _shopService;
        private readonly IMailChimpService _mailChimpService;

        public IndexModel(ICatalogService catalogService, IShopService shopService, IMailChimpService mailChimpService)
        {
            _catalogService = catalogService;
            _shopService = shopService;
            _mailChimpService = mailChimpService;
        }

        public CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

        [ViewData]
        public string MetaDescription { get; set; }
        [ViewData]
        public string Title { get; set; }
        
        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet(CatalogIndexViewModel catalogModel)
        {
            //Shop Stuff
             CatalogModel = await _catalogService.GetCatalogItems(0, null, null, null, null);           

            //DamaStuff
            var config = await _shopService.GetDamaHomePageConfig();
            MetaDescription = config.MetaDescription;
            Title = config.Title;
            CatalogModel.Banners = config.Banners;
        }
        public async Task<IActionResult> OnPostSubscribeNewsletterAsync(string newsletterEmail)
        {
            if (Utils.IsValidEmail(newsletterEmail))
            {
                await _mailChimpService.AddSubscriberAsync(newsletterEmail);
                StatusMessage = "A subscrição foi efetuada com sucesso!";
            }
            else
                StatusMessage = "O email não é valido, por favor tente de novo";
            return RedirectToPage();
        }

    }
}
