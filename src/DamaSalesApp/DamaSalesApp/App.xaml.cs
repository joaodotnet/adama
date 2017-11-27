using DamaSalesApp.Interfaces;
using DamaSalesApp.Models;
using DamaSalesApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DamaSalesApp
{
    public partial class App : Application
    {
        static DamaDataContext database;
        public static DamaDataContext Database
        {
            get
            {
                if (database == null)
                {
                    database = new DamaDataContext(DependencyService.Get<IFileHelper>().GetLocalFilePath("damasales.db3"));
                }

                return database;
            }
        }

        public static MainViewModel ViewModel { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new DamaSalesApp.MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
