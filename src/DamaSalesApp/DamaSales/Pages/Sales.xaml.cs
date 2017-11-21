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

                StackLayout stack = new StackLayout();
                //var image = new Image { Source = "http://via.placeholder.com/100x100" };
                //image.GestureRecognizers.Add(tapGestureRecognizer);
                var box = new BoxView { HeightRequest = 150, WidthRequest = 150, Color = GetCategoryColor(item.Name)};
                box.GestureRecognizers.Add(tapGestureRecognizer);
                stack.Children.Add(box);
                stack.Children.Add(new Label { Text = item.Name, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.CenterAndExpand });
                categoriesList.Children.Add(stack, column, row);
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
                    return new Color(0, 160, 141);
                case "Decoração":
                    return new Color(255, 181, 181);
                case "Design":
                    return new Color(229, 190, 221);
                case "Papelaria":
                    return new Color(237, 218, 196);
                case "Personalizado":
                    return new Color(58, 191, 201);
                default:
                    return new Color();
            }
        }
    }

}
