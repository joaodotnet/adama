using DamaSales.Models;
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

        public Sales()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //categoriesListView.IsRefreshing = true;           

            int row = 0, column = 0;
            foreach (var item in App.ViewModel.Categories)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => {
                    await DisplayAlert("Item Tapped", $"{item.Name} tapped", "OK");
                };

                StackLayout sl = new StackLayout
                {
                    BackgroundColor = GetCategoryColor(item.Name),
                    HeightRequest = 150,
                    WidthRequest = 150
                };
                sl.GestureRecognizers.Add(tapGestureRecognizer);
                sl.Children.Add(new Label { Text = item.Name, TextColor = Color.FromHex("fff"), VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand });
                categoriesList.Children.Add(sl, column, row);
                column++;
                if (column == 4)
                {
                    column = 0;
                    row++;
                }

            }

            this.BindingContext = App.ViewModel;

            //categoriesListView.IsRefreshing = false;

            base.OnAppearing();
        }

        private Color GetCategoryColor(string name)
        {
            switch(name)
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
    }

}
