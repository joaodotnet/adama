using DamaSales.Common;
using DamaSales.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaSales.ViewModels
{
    public class MainViewModel : ObservableBase
    {
        public MainViewModel()
        {
            this.Categories = new ObservableCollection<Category>();
        }

        public string CurrentDate { get; set; }
        public string Place { get; set; }

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return this._categories; }
            set { this.SetProperty(ref this._categories, value); }
        }        

        private bool _isBusy;
        public bool IsBusy
        {
            get { return this._isBusy; }
            set { this.SetProperty(ref this._isBusy, value); }
        }

        public void RefreshCategories()
        {
            this.IsBusy = true;

            var categoriesList = new List<Category>()
            {
                new Category { Id = 1, Name = "Acessórios" },
                new Category { Id = 2, Name = "Decoração" },
                new Category { Id = 3, Name = "Design" },
                new Category { Id = 4, Name = "Papelaria" },
                new Category { Id = 5, Name = "Personalizado" }
            };

            foreach (var item in categoriesList)
            {
                this.Categories.Add(item);
            }

            this.IsBusy = false;
        }

    }
}
