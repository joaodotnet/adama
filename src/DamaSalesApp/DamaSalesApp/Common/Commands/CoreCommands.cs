using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DamaSalesApp.Models;

namespace DamaSalesApp.Common.Commands
{
    public class RefreshCommand : ICommand
    {
        private bool _isBusy = false;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !_isBusy;
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            RefreshAsync();
        }

        private async void RefreshAsync()
        {
            this._isBusy = true;
            this.RaiseCanExecuteChanged();
            App.ViewModel.IsBusy = true;

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
                App.Database.SaveCategoryAsync(item);
            }

            this._isBusy = false;
            this.RaiseCanExecuteChanged();
            App.ViewModel.IsBusy = false;

        }
    }
}
