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
