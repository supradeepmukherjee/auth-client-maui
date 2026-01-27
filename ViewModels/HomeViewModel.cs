using client_maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        //private readonly BiometricService _biometric;
        private readonly AuthLocalService _auth;

        public HomeViewModel( AuthLocalService auth, ApiClient apiClient)
        {
            _apiClient = apiClient;
            //_biometric = biometric;
            _auth = auth;
        }

        [ObservableProperty] string message = "";

        [RelayCommand]
        async Task LoadAsync()
        {
            var (access, _) = await _auth.GetTokenAsync(requireBiometric: true);
            if (access == null)
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

            Message = await _apiClient.GetMeAsync(access) ?? "Error";
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            await _auth.ClearTokensAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
