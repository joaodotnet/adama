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
        public ObservableCollection<string> Items { get; set; }

        public Sales()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            //categoriesListView.IsRefreshing = true;
            var Items = new List<Category>()
            {
                new Category { Id = 1, Name = "Acessórios" },
                new Category { Id = 2, Name = "Decoração" },
                new Category { Id = 3, Name = "Design" },
                new Category { Id = 4, Name = "Papelaria" },
                new Category { Id = 5, Name = "Personalizado" }
            };

            //var listViewModel = new List<CategoryViewModel>();
            //for (int i = 0; i < Items.Count; i = i + 4)
            //{
            //    var items = Items.Skip(i).Take(4).ToList();
            //    listViewModel.Add(new CategoryViewModel
            //    {
            //        Category1Name = items[0].Name,
            //        HasCategory2 = items.Count > 1,
            //        Category2Name = items.Count > 1 ? items[1].Name : string.Empty,
            //        HasCategory3 = items.Count > 2,
            //        Category3Name = items.Count > 2 ? items[2].Name : string.Empty,
            //        HasCategory4 = items.Count > 3,
            //        Category4Name = items.Count > 3 ? items[3].Name : string.Empty,
            //    });
            //}

            //this.BindingContext = listViewModel;

            var model = new CategoryViewModel
            {
                CurrentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                Place = "Feira de Quarteira"
            };

            int row = 0, column = 0;
            foreach (var item in Items)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => {
                    await DisplayAlert("Item Tapped", $"{item.Name} tapped", "OK");
                };

                StackLayout stack = new StackLayout();
                //var image = new Image { Source = "http://via.placeholder.com/100x100" };
                //image.GestureRecognizers.Add(tapGestureRecognizer);
                var box = new BoxView { HeightRequest = 100, WidthRequest = 100, Color = Color.LawnGreen};
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
            //Content = categoriesList;

            //lblDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            //lblPlace.Text = "Feira de Quarteira";

            this.BindingContext = model;

            //categoriesListView.IsRefreshing = false;

            base.OnAppearing();
        }

        

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }

}
