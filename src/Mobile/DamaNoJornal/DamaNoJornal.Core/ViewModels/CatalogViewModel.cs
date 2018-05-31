using DamaNoJornal.Core.Models.Basket;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.Services.Basket;
using DamaNoJornal.Core.Services.Catalog;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private ObservableCollection<CatalogItem> _products;
        private ObservableCollection<CatalogBrand> _brands;
        private CatalogBrand _brand;
        private ObservableCollection<CatalogType> _types;
        private CatalogType _type;
        private ICatalogService _productsService;
        private ISettingsService _settingsService;
        private IBasketService _basketService;
        private IUserService _userService;

        public CatalogViewModel(
            ICatalogService productsService,
            ISettingsService settingsService,
            IBasketService basketService,
            IUserService userService)
        {
            _productsService = productsService;
            _settingsService = settingsService;
            _basketService = basketService;
            _userService = userService;
        }

        public ObservableCollection<CatalogItem> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        public ObservableCollection<CatalogBrand> Brands
        {
            get { return _brands; }
            set
            {
                _brands = value;
                RaisePropertyChanged(() => Brands);
            }
        }

        public CatalogBrand Brand
        {
            get { return _brand; }
            set
            {
                _brand = value;
                RaisePropertyChanged(() => Brand);
                RaisePropertyChanged(() => IsFilter);
            }
        }

        public ObservableCollection<CatalogType> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                RaisePropertyChanged(() => Types);
            }
        }

        public CatalogType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged(() => Type);
                RaisePropertyChanged(() => IsFilter);
            }
        }

        public bool IsFilter { get { return Brand != null || Type != null; } }

        public ICommand AddCatalogItemCommand => new Command<CatalogItem>(async (item) => await AddCatalogItemAsync(item));

        public ICommand FilterCommand => new Command(async () => await FilterAsync());

        public ICommand ClearFilterCommand => new Command(async () => await ClearFilterAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get Catalog, Brands and Types
            Products = await _productsService.GetCatalogAsync();
            Brands = await _productsService.GetCatalogCategoryAsync();
            Types = await _productsService.GetCatalogTypeAsync();

            IsBusy = false;
        }

        private async Task AddCatalogItemAsync(CatalogItem catalogItem)
        {
            // Add new item to Basket
            //MessagingCenter.Send(this, MessageKeys.AddProduct, catalogItem);   
            await NavigationService.NavigateToAsync<BasketViewModel>(catalogItem);

            //var authToken = _settingsService.AuthAccessToken;
            //var userInfo = await _userService.GetUserInfoAsync(authToken);
            //var basketItem = new BasketItem
            //{
            //    ProductId = catalogItem.Id,
            //    ProductName = catalogItem.Name,
            //    PictureUrl = catalogItem.PictureUri,
            //    UnitPrice = catalogItem.Price,
            //    Quantity = 1
            //};

            //await _basketService.AddBasketItemAsync(new CustomerBasket
            //{
            //    BuyerId = userInfo.UserId,
            //    Items = new List<BasketItem> { basketItem }
            //}, authToken);




            //await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 2 });
            //await NavigationService.NavigateToAsync<BasketViewModel>(catalogItem);
            //await NavigationService.RemoveLastFromBackStackAsync();
        }

        private async Task FilterAsync()
        {
            if (Brand == null || Type == null)
            {
                return;
            }

            IsBusy = true;

            // Filter catalog products
            MessagingCenter.Send(this, MessageKeys.Filter);
            Products = await _productsService.FilterAsync(Brand.Id, Type.Id);

            IsBusy = false;
        }

        private async Task ClearFilterAsync()
        {
            IsBusy = true;

            Brand = null;
            Type = null;
            Products = await _productsService.GetCatalogAsync();

            IsBusy = false;
        }
    }
}