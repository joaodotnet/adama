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
        private const string _bgBtnPopout = "#5cc0c0";
        private const string _bgBtnUnPopout = "#929292";
        private ObservableCollection<CatalogBrand> _categories;
        private ObservableCollection<CatalogType> _types;
        private CatalogBrand _category;
        private CatalogType _type;
        private ICatalogService _productsService;
        private bool _isCategoriesListVisible;
        private bool _isTypeListVisible;
        private string _categoryBgButton = _bgBtnPopout;
        private string _typeBgButton = _bgBtnPopout;

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

        public CatalogBrand Category
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged(() => Category);
                RaisePropertyChanged(() => CategoryTextButton);
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
                RaisePropertyChanged(() => TypeTextButton);
            }
        }
        public bool IsCategoriesListVisible
        {
            get
            {
                return _isCategoriesListVisible;
            }
            set
            {
                _isCategoriesListVisible = value;
                RaisePropertyChanged(() => IsCategoriesListVisible);
            }
        }

        public bool IsTypeListVisible
        {
            get
            {
                return _isTypeListVisible;
            }
            set
            {
                _isTypeListVisible = value;
                RaisePropertyChanged(() => IsTypeListVisible);
            }
        }

        public string CategoryTextButton
        {
            get
            {
                return Category == null ? "Categorias" : Category.Name;
            }
            set
            {
                RaisePropertyChanged(() => CategoryTextButton);
            }
        }
        public string CategoryBgButton
        {
            get
            {
                return _categoryBgButton;
            }
            set
            {
                _categoryBgButton = value;
                RaisePropertyChanged(() => CategoryBgButton);
            }
        }

        public string TypeBgButton
        {
            get
            {
                return _typeBgButton; //"#5cc0c0" : "#929292";
            }
            set
            {
                _typeBgButton = value;
                RaisePropertyChanged(() => TypeBgButton);
            }
        }


        public string TypeTextButton
        {
            get
            {
                return Type == null ? "Tipos de Produto" : Type.Description;
            }
            set
            {
                RaisePropertyChanged(() => TypeTextButton);
            }
        }

        public ICommand OnCategoryItemTappedCommand => new Command<CatalogBrand>(async (item) => await FilterByCategoryAsync(item));
        public ICommand OnTypeItemTappedCommand => new Command<CatalogType>( async (item) => await FilterByTypeAsync(item));
        public ICommand CategoryCommand => new Command(ShowCategories);
        public ICommand TypeCommand => new Command(async() => await ShowTypesAsync());

        public ICommand FilterCommand => new Command(async () => await FilterAsync());
        public ICommand ClearFilterCommand => new Command(async () => await ClearFilterAsync());        

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            Categories = await _productsService.GetCatalogCategoryAsync();
            Types = await _productsService.GetCatalogTypeAsync();
            IsCategoriesListVisible = true;
            IsTypeListVisible = false;
            CategoryBgButton = _bgBtnPopout;
            TypeBgButton = _bgBtnUnPopout;
            IsBusy = false;
        }

        private void ShowCategories()
        {
            Type = null;
            Category = null;
            IsCategoriesListVisible = true;
            IsTypeListVisible = false;
            CategoryBgButton = _bgBtnPopout;
            TypeBgButton = _bgBtnUnPopout;
        }
        private async Task ShowTypesAsync()
        {
            IsBusy = true;
            Type = null;
            Category = null;
            IsCategoriesListVisible = false;
            IsTypeListVisible = true;
            CategoryBgButton = _bgBtnUnPopout;
            TypeBgButton = _bgBtnPopout;
            Types = await _productsService.GetCatalogTypeAsync(null);
            IsBusy = false;
        }

        private async Task FilterByCategoryAsync(CatalogBrand item)
        {
            IsBusy = true;
            //var catalogViewModel = ViewModelLocator.Resolve<CatalogViewModel>();
            //catalogViewModel.Brand = item;
            Category = item;
            IsCategoriesListVisible = false;
            IsTypeListVisible = true;
            CategoryBgButton = _bgBtnUnPopout;
            TypeBgButton = _bgBtnPopout;

            //get Types
            Types = await _productsService.GetCatalogTypeAsync(item.Id);

            IsBusy = false;
            //await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 0, ParameterObj = item });
        }

        private async Task FilterByTypeAsync(CatalogType item)
        {
            Type = item;
            //IsTypeListVisible = false;
            //CategoryBgButton = _bgBtnUnPopout;
            //TypeBgButton = _bgBtnUnPopout;

            await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter
            {
                TabIndex = 0,
                ParameterObj = new Tuple<CatalogBrand, CatalogType>(Category, Type)
            });
            await NavigationService.RemoveBackStackAsync();
        }

        private async Task FilterAsync()
        {
            if (Category == null && Type == null)
            {
                await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter
                {
                    TabIndex = 0
                });
            }
            else
            {
                await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter
                {
                    TabIndex = 0,
                    ParameterObj = new Tuple<CatalogBrand, CatalogType>(Category, Type)
                });
            }
            await NavigationService.RemoveBackStackAsync();
        }

        private async Task ClearFilterAsync()
        {
            await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter
            {
                TabIndex = 0
            });
            await NavigationService.RemoveBackStackAsync();
        }
    }
}
