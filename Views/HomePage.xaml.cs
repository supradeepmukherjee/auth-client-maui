using client_maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Views
{
    public partial class HomePage:ContentPage
    {
        private readonly HomeViewModel _vm;

        public HomePage(HomeViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadAsync();
        }
    }
}
