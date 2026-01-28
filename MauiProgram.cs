using client_maui.Services;
using client_maui.ViewModels;
using client_maui.Views;
using Microsoft.Extensions.Logging;

namespace client_maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<AuthHttpHandler>();

            builder.Services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5166");
            })
            .AddHttpMessageHandler<AuthHttpHandler>();

            builder.Services.AddSingleton<BiometricService>();
            builder.Services.AddSingleton<AuthLocalService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<RegisterPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
