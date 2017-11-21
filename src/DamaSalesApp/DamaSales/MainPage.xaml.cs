using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DamaSales
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if(App.ViewModel == null)
            {
                App.ViewModel = new ViewModels.MainViewModel
                {
                    CurrentDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Place = "Feira de Quarteira"
                };

                App.ViewModel.RefreshCategories();
            }
            base.OnAppearing();
        }

        private async void Sales_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Sales());
        }

        private async void Orders_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Orders());
        }
    }
}
