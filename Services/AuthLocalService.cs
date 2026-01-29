using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Services
{
    public class AuthLocalService
    {
        const string KEY_ACCESS = "auth_access_token";
        const string KEY_REFRESH = "auth_refresh_token";
        private const string BiometricEnabledKey = "biometric_enabled";
        private bool _isUnlocked = false;
        private readonly BiometricService _biometric;

        public AuthLocalService(BiometricService biometric)
        {
            _biometric = biometric;
        }

        public async Task SaveTokensAsync(string accessToken, string refreshToken)
        {

                await SecureStorage.SetAsync(KEY_ACCESS, accessToken ?? "");
                await SecureStorage.SetAsync(KEY_REFRESH, refreshToken ?? "");

            _isUnlocked = true;
        }

        public async Task ClearTokensAsync()
        {
            try
            {
                SecureStorage.Remove(KEY_ACCESS);
                SecureStorage.Remove(KEY_REFRESH);
            }
            catch { }
        }

        public async Task<(string? AccessToken, string? RefreshToken)> GetTokenAsync(bool requireBiometric)
        {
            var access = await SecureStorage.GetAsync(KEY_ACCESS);
            var refresh = await SecureStorage.GetAsync(KEY_REFRESH);

            if (string.IsNullOrEmpty(refresh))
                return (null, null);

            if (_isUnlocked && !string.IsNullOrEmpty(access))
                return (access, refresh);

            if (requireBiometric)
            {
                var ok = await _biometric.AuthenticateAsync();
                if (!ok) return (null, null);

                _isUnlocked = true;
                return (access, refresh);
            }

            return (null, refresh);
        }

        public async Task<string?> GetRefreshTokenSilentlyAsync()
        {
            var refresh = await SecureStorage.GetAsync(KEY_REFRESH);

            if (!string.IsNullOrEmpty(refresh)) return refresh;
            return await SecureStorage.GetAsync(KEY_REFRESH);
        }

        public async Task<bool> HasStoredTokensAsync()
        {
            var r = await SecureStorage.GetAsync(KEY_REFRESH);
            return !string.IsNullOrEmpty(r);
        }
    }
}
