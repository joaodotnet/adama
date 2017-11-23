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
            this.ProductTypes = new ObservableCollection<ProductType>();
            this.Products = new ObservableCollection<Product>();
            this.BasketItems = new ObservableCollection<BasketItem>();
        }

        public string CurrentDate { get; set; }
        public string Place { get; set; }

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return this._categories; }
            set { this.SetProperty(ref this._categories, value); }
        }

        private ObservableCollection<ProductType> _productTypes;
        public ObservableCollection<ProductType> ProductTypes
        {
            get { return this._productTypes; }
            set { this.SetProperty(ref this._productTypes, value); }
        }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return this._products; }
            set { this.SetProperty(ref this._products, value); }
        }

        private ObservableCollection<BasketItem> _basketItems;
        public ObservableCollection<BasketItem> BasketItems
        {
            get { return this._basketItems; }
            set { this.SetProperty(ref this._basketItems, value); }
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

        public void RefreshProductTypes()
        {
            this.IsBusy = true;

            var productTypeList = new List<ProductType>()
            {
                new ProductType { Id = 1, Code = "CAT", Description = "Carteira", CategoryId = 1 },
                new ProductType { Id = 2, Code = "BLS", Description = "Bolsa", CategoryId = 1 },
                new ProductType { Id = 3, Code = "PCH", Description = "Porta Chaves", CategoryId = 1 },
                new ProductType { Id = 4, Code = "ALM", Description = "Almofada", CategoryId = 2 },
                new ProductType { Id = 5, Code = "COL", Description = "Convites", CategoryId = 3 },
            };

            foreach (var item in productTypeList)
            {
                this.ProductTypes.Add(item);
            }

            this.IsBusy = false;
        }

        public void RefreshProducts()
        {
            this.IsBusy = true;

            var products = new List<Product>()
            {
                new Product { ProductTypeId = 1, Name = "Carteira Dama Sevilha", Price = 5.50m  },
                new Product { ProductTypeId = 2, Name = "Bolsa Dama Cadiz", Price = 3.50m  },
                new Product { ProductTypeId = 3, Name = "Porta Chaves Faro", Price = 2.50m  },
                new Product { ProductTypeId = 1, Name = "Carteira Dama Cadiz", Price = 5.50m  },
                new Product { ProductTypeId = 1, Name = "Carteira Dama Huelva", Price = 5.50m  },
            };

            foreach (var item in products)
            {
                this.Products.Add(item);
            }

            this.IsBusy = false;
        }


    }
}
