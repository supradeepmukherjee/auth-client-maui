using client_maui.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace client_maui
{
    public partial class App : Application
    {
        private readonly AuthLocalService _auth;
        private readonly BiometricService _biometric;

        public App(AuthLocalService auth, BiometricService biometric)
        {
            InitializeComponent();
            _auth = auth;
            _biometric = biometric;
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var shell = IPlatformApplication.Current.Services
            .GetRequiredService<AppShell>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await HandleStartupAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Startup error: {ex}");
                }
            });

            return new Window(shell);
        }

        private async Task HandleStartupAsync()
        {
            Debug.WriteLine("Startup");

            var hasTokens = await _auth.HasStoredTokensAsync();
            Debug.WriteLine($"Has tokens: {hasTokens}");

            if (!hasTokens)
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

            var (access, _) = await _auth.GetTokenAsync(requireBiometric: true);

            if (string.IsNullOrEmpty(access))
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

            await Shell.Current.GoToAsync("//home");
        }
    }
}