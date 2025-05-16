using APP_FarmaciaChavarria.Models.AuthRequest;
using APP_FarmaciaChavarria.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FarmaciaChavarria.Services;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;


namespace FarmaciaChavarria.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        private string usuario = string.Empty;

        [ObservableProperty]
        private string contraseña = string.Empty;

        [ObservableProperty]
        private string mensaje = string.Empty;

        [RelayCommand]
        public async Task IniciarSesion()
        {
            try
            {
                var request = new LoginRequest
                {
                    nombre = Usuario,
                    pin = Convert.ToInt32(Contraseña)
                };

                var response = await _authService.LoginUser(request);

                if (!(response == ""))
                {
                    Mensaje = "Has iniciado sesión con éxito";
                    Preferences.Default.Set("JwtToken", response);
                    Preferences.Default.Set("TokenCreatedAt", DateTime.UtcNow.ToString());

                }
                else
                {
                    Mensaje = "Error: No se pudo iniciar sesión.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error: {ex.Message}";
            }
        }

    }
}
