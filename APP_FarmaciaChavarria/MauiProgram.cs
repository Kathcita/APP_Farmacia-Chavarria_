using APP_FarmaciaChavarria.Components.Pages.UiCategorias;
using APP_FarmaciaChavarria.Service;
using APP_FarmaciaChavarria.ViewModels;
using APP_FarmaciaChavarria.ViewModels.CategoriaViewModels;
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
#if WINDOWS
    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
#endif

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

#if ANDROID
        string baseAddress = "https://10.0.2.2:7053/";
#else
        string baseAddress = "https://localhost:7053/";
#endif

            builder.Services.AddSingleton(sp => new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            });

            builder.Services.AddScoped<TokenHandlerService>();

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddTransient<AuthService>();
            builder.Services.AddSingleton<ProductoService>();
            builder.Services.AddSingleton<CategoriaService>();
            builder.Services.AddSingleton<LaboratorioService>();
            builder.Services.AddSingleton<FacturaService>();
            builder.Services.AddSingleton<CompraService>();
            builder.Services.AddSingleton<UsuarioService>();
            builder.Services.AddSingleton<ProveedorService>();

            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<InventarioViewModel>();
            builder.Services.AddSingleton<ProductoDetalleViewModel>();
            builder.Services.AddSingleton<CrearProductoViewModel>();
            builder.Services.AddSingleton<FacturacionViewModel>();
            builder.Services.AddSingleton<CrearLaboratorioViewModel>();
            builder.Services.AddSingleton<ActualizarProductoViewModel>();
            builder.Services.AddSingleton<CategoriaViewModel>();
            builder.Services.AddSingleton<DetalleCategoriaViewModel>();
            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddSingleton<MedicamentosPorCaducarViewModel>();
            builder.Services.AddSingleton<CompraViewModel>();

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
