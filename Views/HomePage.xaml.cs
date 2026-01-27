using client_maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Views
{
    public partial class HomePage:ContentPage
    {
        public HomePage(HomeViewModel vm)
        {
            InitializeComponent();
            BindingContext=vm;
        }
    }
}
