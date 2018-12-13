using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesWeb.ViewModels;
using SalesWeb.Interfaces;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SalesWeb.Pages.Basket
{
    public class IndexModel : PageModel
    {
        private readonly IBasketService _basketService;
        private const string _basketSessionKey = "basketId";
        private readonly IUriComposer _uriComposer;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private string _username = null;
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly ILogger<BasketViewModel> _logger;

        public IndexModel(IBasketService basketService,
            IBasketViewModelService basketViewModelService,
            IUriComposer uriComposer,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            _basketService = basketService;
            _uriComposer = uriComposer;
            _signInManager = signInManager;
            _basketViewModelService = basketViewModelService;
            _logger = loggerFactory.CreateLogger<BasketViewModel>();
        }

        public BasketViewModel BasketModel { get; set; } = new BasketViewModel();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet()
        {
            await SetBasketModelAsync();
        }

        public async Task<IActionResult> OnPost(CatalogItemViewModel productDetails, string catName = null, string typeName = null)
        {
            if (productDetails?.CatalogItemId == null)
            {
                return RedirectToPage("/Index");
            }
            await SetBasketModelAsync();

            var options = _basketService.GetFirstOptionFromAttribute(productDetails.CatalogItemId);

            await _basketService.AddItemToBasket(BasketModel.Id, productDetails.CatalogItemId, productDetails.Price, 1, options.Item1, options.Item2, options.Item3);

            await SetBasketModelAsync();

            StatusMessage = $"Adicionado {productDetails.CatalogItemName} ao carrinho!";
            if (!string.IsNullOrEmpty(catName) && string.IsNullOrEmpty(typeName))
                return RedirectToPage("/Category/Index", new { id = catName });
            else if (!string.IsNullOrEmpty(catName) && !string.IsNullOrEmpty(typeName))
                return RedirectToPage("/Category/Type/Index", new { cat = catName, type = typeName });
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAddToBasketAsync(ProductViewModel productDetails)
        {
            if (productDetails?.ProductId == null)
            {
                return RedirectToPage("/Index");
            }
            await SetBasketModelAsync();

            //Get Attributes ids
            var attrIds = new List<int>();
            if (productDetails.Attributes?.Count > 0)
                attrIds = productDetails.Attributes.Select(x => x.Selected).ToList();
            var options = GetOptionsFromAttributes(attrIds);
            var price = string.IsNullOrEmpty(productDetails.NameInput) ? productDetails.ProductPrice : productDetails.ProductPrice + productDetails.CustomizePrice.Value;
            await _basketService.AddItemToBasket(BasketModel.Id, productDetails.ProductId, price, productDetails.ProductQuantity, options.Item1, options.Item2, options.Item3, productDetails.NameInput, productDetails.CustomizePictureFileName );

            await SetBasketModelAsync();

            return RedirectToPage();
        }
        
        public async Task OnPostUpdate(Dictionary<string,int> items)
        {
            await SetBasketModelAsync();
            await _basketService.SetQuantities(BasketModel.Id, items);

            await SetBasketModelAsync();
        }

        public async Task<IActionResult> OnPostCheckout(Dictionary<string, int> items)
        {
            await SetBasketModelAsync();
            await _basketService.SetQuantities(BasketModel.Id, items);
            await SetBasketModelAsync();

            return RedirectToPage("/Basket/Checkout");
        }

        public async Task OnPostRemove(int id)
        {
            await SetBasketModelAsync();
            await _basketService.DeleteItem(BasketModel.Id, id);
            await SetBasketModelAsync();
        }

        private (int?, int?, int?) GetOptionsFromAttributes(List<int> attrIds)
        {
            var options = (default(int?), default(int?), default(int?));
            for (int i = 0; i < attrIds?.Count; i++)
            {
                if (i == 0)
                    options.Item1 = attrIds[i];
                if (i == 1)
                    options.Item2 = attrIds[i];
                if (i == 2)
                    options.Item3 = attrIds[i];
            }
            return options;
        }


        private async Task SetBasketModelAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(User.Identity.Name);
            }
            else
            {
                GetOrSetBasketCookieAndUserName();                
                BasketModel = await _basketViewModelService.GetOrCreateBasketForUser(_username);
            }
        }

        private void GetOrSetBasketCookieAndUserName()
        {
            if (Request.Cookies.ContainsKey(Constants.BASKET_COOKIENAME))
            {                
                _username = Request.Cookies[Constants.BASKET_COOKIENAME];                
            }
            if (_username != null) return;

            _username = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Today.AddYears(1),
                IsEssential = true
            };
            Response.Cookies.Append(Constants.BASKET_COOKIENAME, _username, cookieOptions);
            
        }
    }
}
