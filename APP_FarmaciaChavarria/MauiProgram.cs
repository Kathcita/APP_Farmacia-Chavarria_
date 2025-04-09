using FarmaciaChavarria.Services;
using FarmaciaChavarria.ViewModels;
using Microsoft.Extensions.Logging;

namespace APP_FarmaciaChavarria
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
                });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7053/") });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddTransient<AuthService>();
            builder.Services.AddSingleton<LoginViewModel>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif




            return builder.Build();
        }
    }
}
