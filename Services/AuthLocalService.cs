using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Services
{
    public class AuthLocalService
    {
        private const string AccessKey = "access_token";
        private const string RefreshKey = "refresh_token";
        private const string BiometricEnabledKey = "biometric_enabled";

        public async Task SaveTokensAsync(string accessToken, string refreshToken)
        {
            await SecureStorage.SetAsync(AccessKey, accessToken);
            await SecureStorage.SetAsync(RefreshKey, refreshToken);
        }

        public async Task<bool> HasTokensAsync()
        {
            try
            {
                var a = await SecureStorage.GetAsync(AccessKey);
                var r = await SecureStorage.GetAsync(RefreshKey);
                return !string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(r);
            }
            catch { return false; }
        }

        public async Task<(string? access, string? refresh)> GetTokenAsync(bool requireBiometric = false)
        {
            //if (requireBiometric)
            //{
            //    var auth = await CrossFingerprint.Current.IsAvailableAsync(true);
            //    if (!auth) throw new InvalidOperationException("Biometric/auth not available");

            //    var result = await CrossFingerprint.Current.AuthenticateAsync(new AuthenticationRequestConfiguration("Unlock", "Use biometric to unlock token"));
            //    if (!result.Authenticated) return (null, null);
            //}

            var access = await SecureStorage.GetAsync(AccessKey);
            var refresh = await SecureStorage.GetAsync(RefreshKey);
            return (access, refresh);
        }

        public async Task ClearTokensAsync()
        {
             SecureStorage.Remove(AccessKey);
             SecureStorage.Remove(RefreshKey);
        }

        public Task EnableBiometricAsync(bool enable)=>SecureStorage.SetAsync(BiometricEnabledKey, enable ? "1" : "0");

        public async Task<bool> IsBiometricEnabledAsync()
        {
            var v = await SecureStorage.GetAsync(BiometricEnabledKey);
            return v == "1";
        }
    }
}
