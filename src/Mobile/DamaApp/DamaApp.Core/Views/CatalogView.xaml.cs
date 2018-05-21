﻿using DamaApp.Core.ViewModels;
using DamaApp.Core.ViewModels.Base;
using SlideOverKit;
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace DamaApp.Core.Views
{
    public partial class CatalogView : ContentPage, IMenuContainerPage
    {
        private FiltersView _filterView = new FiltersView();

        public CatalogView()
        {
            InitializeComponent();

            SlideMenu = _filterView;

            MessagingCenter.Subscribe<CatalogViewModel>(this, MessageKeys.Filter, (sender) =>
            {
                Filter();
            });
        }

        public Action HideMenuAction
        {
            get;
            set;
        }

        public Action ShowMenuAction
        {
            get;
            set;
        }

        public SlideMenuView SlideMenu
        {
            get;
            set;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            _filterView.BindingContext = BindingContext;            

        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            if (SlideMenu.IsShown)
            {
                HideMenuAction?.Invoke();
            }
            else
            {
                ShowMenuAction?.Invoke();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = BindingContext as CatalogViewModel;
        }
    }
}