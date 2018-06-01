using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Models.Navigation;
using DamaNoJornal.Core.Services.Catalog;
using DamaNoJornal.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DamaNoJornal.Core.ViewModels
{
    public class CatalogFilterViewModel : ViewModelBase
    {
        private ObservableCollection<CatalogBrand> _categories;
        private ObservableCollection<CatalogType> _types;
        private ICatalogService _productsService;

        public CatalogFilterViewModel(
            ICatalogService productsService)
        {
            _productsService = productsService;
        }

        public ObservableCollection<CatalogBrand> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
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

        public ICommand FilterByCategoryCommand => new Command<CatalogBrand>(async (item) => await FilterByCategoryAsync(item));        

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            Categories = await _productsService.GetCatalogCategoryAsync();
            Types = await _productsService.GetCatalogTypeAsync(); 

            IsBusy = false;
        }

        private async Task FilterByCategoryAsync(CatalogBrand item)
        {
            //var catalogViewModel = ViewModelLocator.Resolve<CatalogViewModel>();
            //catalogViewModel.Brand = item;
            await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 0, ParameterObj = item });
        }
    }
}
