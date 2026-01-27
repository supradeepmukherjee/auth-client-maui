using client_maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        public RegisterViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [ObservableProperty] string email = "";
        [ObservableProperty] string phone = "";
        [ObservableProperty] string password = "";
        [ObservableProperty] string displayName = "";
        [ObservableProperty] string status = "";

        [RelayCommand]
        async Task RegisterAsync()
        {
            var ok = await _apiClient.RegisterAsync(Email, Phone, Password, DisplayName);
            if (!ok)
            {
                Status = "Registration failed";
                return;
            }

            await Shell.Current.GoToAsync("//login");
        }
    }
}
