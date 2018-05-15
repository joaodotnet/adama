using DamaApp.Core.Extensions;
using DamaApp.Core.Models.Catalog;
using DamaApp.Core.Services.Catalog;
using DamaApp.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaApp.Core.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private ObservableCollection<CatalogItem> _products;
        private ObservableCollection<Tuple<CatalogItem,CatalogItem>> _tupleProducts;
        private ObservableCollection<CatalogBrand> _brands;
        private CatalogBrand _brand;
        private ObservableCollection<CatalogType> _types;
        private CatalogType _type;
        private ICatalogService _productsService;

        public CatalogViewModel(ICatalogService productsService)
        {
            _productsService = productsService;
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

        public ObservableCollection<Tuple<CatalogItem,CatalogItem>> TupleProducts
        {
            get { return _tupleProducts; }
            set
            {
                _tupleProducts = value;
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

        public ICommand AddCatalogItemCommand => new Command<CatalogItem>(AddCatalogItem);

        public ICommand FilterCommand => new Command(async () => await FilterAsync());

		public ICommand ClearFilterCommand => new Command(async () => await ClearFilterAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get Catalog, Brands and Types
            //var products = await _productsService.GetCatalogAsync();
            //List<Tuple<CatalogItem, CatalogItem>> tupleList = new List<Tuple<CatalogItem, CatalogItem>>();
            //for (int i = 0; i < products.Count; i = i + 2)
            //{
            //    CatalogItem prod1 = products[i];
            //    CatalogItem prod2 = new CatalogItem();                
            //    if (products.Count < i + 1)
            //        prod2 = products[i+1];
            //    tupleList.Add(new Tuple<CatalogItem, CatalogItem>(prod1, prod2));
            //}
            //TupleProducts = tupleList.ToObservableCollection();
            TupleProducts = new ObservableCollection<Tuple<CatalogItem, CatalogItem>>();
            Products = await _productsService.GetCatalogAsync();
            for (int i = 0; i < Products.Count; i += 2)
            {
                CatalogItem item1 = Products[i];
                CatalogItem item2 = i + 1 < Products.Count ? Products[i + 1] : null;

                TupleProducts.Add(new Tuple<CatalogItem,CatalogItem>(item1, item2));
            }
            Brands = await _productsService.GetCatalogBrandAsync();
            Types = await _productsService.GetCatalogTypeAsync();

            IsBusy = false;
        }

        private void AddCatalogItem(CatalogItem catalogItem)
        {
            // Add new item to Basket
            MessagingCenter.Send(this, MessageKeys.AddProduct, catalogItem);
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