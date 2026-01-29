using client_maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace client_maui.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly BiometricService _biometric;
        private readonly AuthLocalService _auth;
        bool _hasUnlocked = false;

        public HomeViewModel( AuthLocalService auth, ApiClient apiClient, BiometricService biometric)
        {
            _apiClient = apiClient;
            _biometric = biometric;
            _auth = auth;
        }

        [ObservableProperty] string name;
        [ObservableProperty] string email;
        [ObservableProperty] string phone;
        [ObservableProperty] string roles;
        [ObservableProperty] string provider;

        [RelayCommand]
        public async Task LoadAsync()
        {
            var (access, _) = await _auth.GetTokenAsync(requireBiometric: false);
            if (access == null)
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

            var json = await _apiClient.GetMeAsync(access);
            if (string.IsNullOrWhiteSpace(json)) return;
            //Name = json;
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Name = root.GetProperty("name").GetString();
            Email = root.GetProperty("email").GetString();
            Phone = root.GetProperty("phone").GetString();
            Provider = root.GetProperty("provider").GetString() ?? "local";

            Roles = string.Join(", ",
                root.GetProperty("roles").EnumerateArray()
                    .Select(r => r.GetString()));
        }

        [RelayCommand]
        public async Task AuthenticateAndLoadAsync()
        {
            var (access, _) = await _auth.GetTokenAsync(requireBiometric: true);
            if (string.IsNullOrEmpty(access))
            {
                await _auth.ClearTokensAsync();
                await Shell.Current.GoToAsync("//login");
                return;
            }

            var json = await _apiClient.GetMeAsync(access);
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            await _auth.ClearTokensAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
