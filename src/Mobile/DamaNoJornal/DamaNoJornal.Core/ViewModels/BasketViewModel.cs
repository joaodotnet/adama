using DamaNoJornal.Core.Models.Basket;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Services.Basket;
using DamaNoJornal.Core.Services.Settings;
using DamaNoJornal.Core.Services.User;
using DamaNoJornal.Core.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class BasketViewModel : ViewModelBase
    {
        private int _badgeCount;
        private ObservableCollection<BasketItem> _basketItems;
        private decimal _total;

        private ISettingsService _settingsService;
        private IBasketService _basketService;
        private IUserService _userService;

        public BasketViewModel(
            ISettingsService settingsService,
            IBasketService basketService,
            IUserService userService)
        {
            _settingsService = settingsService;
            _basketService = basketService;
            _userService = userService;
        }

        public int BadgeCount
        {
            get { return _badgeCount; }
            set
            {
                _badgeCount = value;
                RaisePropertyChanged(() => BadgeCount);
            }
        }

        public ObservableCollection<BasketItem> BasketItems
        {
            get { return _basketItems; }
            set
            {
                _basketItems = value;
                RaisePropertyChanged(() => BasketItems);
            }
        }

        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged(() => Total);
            }
        }

        public ICommand AddCommand => new Command<BasketItem>((item) => AddItem(item));

        public ICommand CheckoutCommand => new Command(async () => await CheckoutAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            if (BasketItems == null)
                BasketItems = new ObservableCollection<BasketItem>();

            var authToken = _settingsService.AuthAccessToken;
            var userInfo = await _userService.GetUserInfoAsync(authToken);

            // Update Basket
            var basket = await _basketService.GetBasketAsync(userInfo.UserId, authToken);

            if (basket != null && basket.Items != null && basket.Items.Any())
            {
                BadgeCount = 0;
                BasketItems.Clear();

                foreach (var basketItem in basket.Items)
                {
                    BadgeCount += basketItem.Quantity;
                    AddBasketItem(basketItem);
                }
            }
            System.Diagnostics.Debug.WriteLine("############## Unsubcribe 1 #################");
            MessagingCenter.Unsubscribe<CatalogViewModel, CatalogItem>(this, MessageKeys.AddProduct);
            System.Diagnostics.Debug.WriteLine("############## Subcribe 1 #################");
            MessagingCenter.Subscribe<CatalogViewModel, CatalogItem>(this, MessageKeys.AddProduct, async (sender, arg) =>
            {
                System.Diagnostics.Debug.WriteLine("############## Executing Subscribe Method #################");
                BadgeCount++;

                await AddCatalogItemAsync(arg);
            });

            await base.InitializeAsync(navigationData);
        }

        private async Task AddCatalogItemAsync(CatalogItem item)
        {
            var basketItem = new BasketItem
            {
                ProductId = item.Id,
                ProductName = item.Name,
                PictureUrl = item.PictureUri,
                UnitPrice = item.Price,
                Quantity = 1
            };
            BasketItems.Add(basketItem);

            ReCalculateTotal();

            var authToken = _settingsService.AuthAccessToken;
            var userInfo = await _userService.GetUserInfoAsync(authToken);

            var listToSend = new List<BasketItem>();
            listToSend.Add(basketItem);
            await _basketService.AddBasketItemAsync(new CustomerBasket
            {
                BuyerId = userInfo.UserId,
                Items = listToSend
            }, authToken);
        }

        private void AddItem(BasketItem item)
        {
            BadgeCount++;
            AddBasketItem(item);
            RaisePropertyChanged(() => BasketItems);
        }

        private void AddBasketItem(BasketItem item)
        {
            BasketItems.Add(item);
            ReCalculateTotal();
        }

        private void ReCalculateTotal()
        {
            Total = 0;

            if (BasketItems == null)
            {
                return;
            }

            foreach (var orderItem in BasketItems)
            {
                Total += (orderItem.Quantity * orderItem.UnitPrice);
            }

            //var authToken = _settingsService.AuthAccessToken;
            //var userInfo = await _userService.GetUserInfoAsync(authToken);

            //await _basketService.UpdateBasketAsync(new CustomerBasket
            //{
            //    BuyerId = userInfo.UserId,
            //    Items = BasketItems.ToList()
            //}, authToken);
        }

        private async Task CheckoutAsync()
        {
            if (BasketItems.Any())
            {
                await NavigationService.NavigateToAsync<CheckoutViewModel>(BasketItems);
            }
        }
    }
}