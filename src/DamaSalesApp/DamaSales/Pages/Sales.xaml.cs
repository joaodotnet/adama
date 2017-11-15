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

            this.BindingContext = Items;

            //categoriesListView.IsRefreshing = false;

            base.OnAppearing();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
