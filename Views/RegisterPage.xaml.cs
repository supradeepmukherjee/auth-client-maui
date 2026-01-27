using client_maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}