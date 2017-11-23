using DamaSales.Models;
using DamaSales.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DamaSales.Pages
{
    public partial class Sales : ContentPage
    {
        public int? CategoryId { get; set; }
        public int? ProductTypeId { get; set; }

        public Sales(int? categoryId = null, int? productTypeId = null)
        {            
            InitializeComponent();

            CategoryId = categoryId;
            ProductTypeId = productTypeId;
        }

        protected override void OnAppearing()
        {
            //categoriesListView.IsRefreshing = true;           

            int row = 0, column = 0;

            if (!CategoryId.HasValue)
            {
                foreach (var item in App.ViewModel.Categories)
                {
                    var tapGestureRecognizer = CreateTapGesture(item.Id);
                    StackLayout sl = CreateChild(ViewType.CATEGORY, item.Name, tapGestureRecognizer);
                    AddToGrid(ref row, ref column, sl);
                }
            }
            else if(!ProductTypeId.HasValue)
            {
                foreach (var item in App.ViewModel.ProductTypes.Where(x => x.CategoryId == CategoryId.Value).ToList())
                {
                    var tapGestureRecognizer = CreateTapGesture(item.CategoryId, item.Id);
                    StackLayout sl = CreateChild(ViewType.PRODUCT_TYPE, item.Description, tapGestureRecognizer);
                    AddToGrid(ref row, ref column, sl);
                }
            }
            else
            {
                foreach (var item in App.ViewModel.Products.Where(x => x.ProductTypeId == ProductTypeId.Value).ToList())
                {
                    var tapGestureRecognizer = CreateTapGesture(CategoryId, ProductTypeId, item);
                    StackLayout sl = CreateChild(ViewType.PRODUCT, item.Name, tapGestureRecognizer);
                    AddToGrid(ref row, ref column, sl);
                }
            }

            this.BindingContext = App.ViewModel;

            //categoriesListView.IsRefreshing = false;

            base.OnAppearing();
        }

        private void AddToGrid(ref int row, ref int column, StackLayout sl)
        {
            categoriesList.Children.Add(sl, column, row);
            column++;
            if (column == 4)
            {
                column = 0;
                row++;
            }
        }

        private StackLayout CreateChild(ViewType type, string name, TapGestureRecognizer tapGestureRecognizer)
        {
            StackLayout sl = new StackLayout
            {
                BackgroundColor = GetColor(type, name),
                HeightRequest = 150,
                WidthRequest = 150
            };
            sl.GestureRecognizers.Add(tapGestureRecognizer);
            sl.Children.Add(new Label { Text = name, TextColor = Color.FromHex("fff"), VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand });
            return sl;
        }

        private TapGestureRecognizer CreateTapGesture(int? categoryId, int? productTypeId = null, Product product = null)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();

            if(product != null)
            {                
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
                    App.ViewModel.BasketItems.Add(new BasketItem
                    {
                        Product = product,
                        DisplayName = product.Name,
                        Quantity = 1,
                        UnitPrice = product.Price
                    });
                    await DisplayAlert("Carrinho", $"O produto {product.Name} foi adicionado", "OK");
                };
            }
            if (productTypeId.HasValue && categoryId.HasValue)
            {
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
                    await Navigation.PushAsync(new Pages.Sales(categoryId,productTypeId));
                };
            }
            else if (categoryId.HasValue)
            {
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
                    await Navigation.PushAsync(new Pages.Sales(categoryId));
                };
            }
            
            return tapGestureRecognizer;
        }

        private Color GetColor(ViewType type, string name)
        {
            if (type == ViewType.CATEGORY)
            {
                switch (name)
                {
                    case "Acessórios":
                        return Color.FromHex("00a08d");
                    case "Decoração":
                        return Color.FromHex("ffb5b5");
                    case "Design":
                        return Color.FromHex("e5bedd");
                    case "Papelaria":
                        return Color.FromHex("eddac4");
                    case "Personalizado":
                        return Color.FromHex("3abfc9");
                    default:
                        return Color.FromHex("000000");
                }
            }
            else if (type == ViewType.PRODUCT_TYPE)
                return Color.FromHex("049994");
            else if (type == ViewType.PRODUCT)
                return Color.FromHex("042251");
            return Color.FromHex("000000");
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = e.Item as BasketItem;

            var res = await DisplayAlert("Carrinho", $"Deseja remover o produto {product.DisplayName}?", "Sim", "Não");
            if (res)
                App.ViewModel.BasketItems.Remove(product);
        }
    }

}
