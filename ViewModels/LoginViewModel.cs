using client_maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.ViewModels
{
    public partial class LoginViewModel:ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly BiometricService _biometric;
        private readonly AuthLocalService _auth;

        public LoginViewModel(AuthLocalService auth, ApiClient apiClient, BiometricService biometric)
        {
            _apiClient = apiClient;
            _biometric = biometric;
            _auth= auth;
        }

        [ObservableProperty]
        string identifier = "";

        [ObservableProperty]
        string password = "";

        [ObservableProperty]
        string status = "";

        [RelayCommand]
        public async Task LoginAsync()
        {
            Status = "Logging in...";
            var token = await _apiClient.LoginAsync(Identifier, Password);
            if (token == null)
            {
                Status = "Login failed";
                return;
            }
            await _auth.SaveTokensAsync(token.AccessToken, token.RefreshToken);
            Status = "Logged in";
            await Shell.Current.GoToAsync("//home");
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
        }

        [RelayCommand]
        public async Task SsoGoogleAsync()
        {
            Status = "Starting Google sign-in...";
            try
            {
                var clientId = "289098979106-isvq84aed7dv4io6l83rgegranvausf3.apps.googleusercontent.com";
                var redirectUri = $"com.companyname.clientmaui://oauth2redirect";
                var scope = "openid email profile";
                var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=id_token%20token&scope={Uri.EscapeDataString(scope)}&nonce={Guid.NewGuid()}&prompt=select_account";

                var authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(authUrl),
                    new Uri(redirectUri));

                if (authResult.Properties.TryGetValue("id_token", out var idToken))
                {
                    var token = await _apiClient.ExternalLoginAsync("google", idToken);
                    if (token != null)
                    {
                        await _auth.SaveTokensAsync(token.AccessToken, token.RefreshToken);
                        Status = "SSO login successful";
                        return;
                    }
                    Status = "Server exchange failed";
                }
                else
                {
                    Status = "id_token not found in response";
                }
            }
            catch (Exception ex)
            {
                Status = $"SSO error: {ex.Message}";
            }
        }

        [RelayCommand]
        async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//register");
        }

        [RelayCommand]
        async Task BiometricLoginAsync()
        {
            var ok = await _biometric.AuthenticateAsync();
            if (!ok) return;

            var token = await _auth.GetTokenAsync();
            if (token.access is not null)
                await Shell.Current.GoToAsync("home");
        }
    }
}
