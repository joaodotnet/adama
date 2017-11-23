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
        //public ObservableCollection<string> Items { get; set; }
        //public ScreenModeType ScreenMode { get; set; }
        public int? CategoryId { get; set; }

        public Sales(int? categoryId = null)
        {            
            InitializeComponent();

            CategoryId = categoryId;
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
            else
            {
                foreach (var item in App.ViewModel.ProductTypes.Where(x => x.CategoryId == CategoryId.Value).ToList())
                {
                    var tapGestureRecognizer = CreateTapGesture(null);
                    StackLayout sl = CreateChild(ViewType.PRODUCT_TYPE, item.Description, tapGestureRecognizer);
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

        private TapGestureRecognizer CreateTapGesture(int? categoryId, string productTypeName = null)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            if(categoryId.HasValue)
            {
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
                    await Navigation.PushAsync(new Pages.Sales(categoryId));
                };
            }
            else
            {
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
                    await DisplayAlert("Item Tapped", $"{productTypeName} tapped", "OK");
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
                return Color.FromHex("cef442");
            return Color.FromHex("000000");
        }
    }

}
