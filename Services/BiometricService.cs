using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace client_maui.Services
{
    public class BiometricService
    {
        public async Task<bool> AuthenticateAsync()
        {
            var availability = await CrossFingerprint.Current.GetAvailabilityAsync();

            if (availability != FingerprintAvailability.Available)
                return false;

            var request = new AuthenticationRequestConfiguration(
                "Unlock app",
                "Authenticate to continue");

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            return result.Authenticated;
        }
    }
}
