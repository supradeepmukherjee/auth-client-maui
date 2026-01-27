using client_maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
