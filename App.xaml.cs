using client_maui.Services;
using Microsoft.Extensions.DependencyInjection;

namespace client_maui
{
    public partial class App : Application
    {
        public App(AuthLocalService auth, BiometricService biometric)
        {
            InitializeComponent();

            //Dispatcher.Dispatch(async () =>
            //{
            //    var token = await auth.GetTokenAsync();

            //    if (token.access is not null)
            //    {
            //        var ok = await biometric.AuthenticateAsync();
            //        if (ok)
            //        {
            //            await Shell.Current.GoToAsync("home");
            //            return;
            //        }
            //    }
            //});
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}