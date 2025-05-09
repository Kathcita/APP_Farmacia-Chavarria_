using APP_FarmaciaChavarria.ViewModels;
using APP_FarmaciaChavarria.ViewModels.Reportes;
using FarmaciaChavarria.Services;
using FarmaciaChavarria.ViewModels;
using Microsoft.Extensions.Logging;
using Radzen;

namespace APP_FarmaciaChavarria
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
            builder.Services.AddSingleton<ProductoService>();
            builder.Services.AddSingleton<CategoriaService>();
            builder.Services.AddSingleton<LaboratorioService>();
            builder.Services.AddSingleton<FacturaService>();
            builder.Services.AddSingleton<UsuarioService>();

            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<InventarioViewModel>();
            builder.Services.AddSingleton<ProductoDetalleViewModel>();
            builder.Services.AddSingleton<CrearProductoViewModel>();
            builder.Services.AddSingleton<CrearLaboratorioViewModel>();
            builder.Services.AddSingleton<ActualizarProductoViewModel>();

            builder.Services.AddSingleton<ReporteVentasViewModel>();
            builder.Services.AddSingleton<ReporteVentasLaboratoriosViewModel>();
            builder.Services.AddSingleton<ReporteVentasCategoriasViewModel>();
            builder.Services.AddSingleton<ReporteVentasProductoViewModel>();
            builder.Services.AddSingleton<ReporteMedicamentosEscasosViewModel>();

            builder.Services.AddRadzenComponents();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif




            return builder.Build();
        }
    }
}
