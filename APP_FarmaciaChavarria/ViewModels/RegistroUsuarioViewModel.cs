using APP_FarmaciaChavarria.Models.ModelsDTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.ViewModels
{
    public class RegistroUsuarioViewModel
    {
        private readonly HttpClient _http;

        public string Nombre { get; set; }
        public string Rol { get; set; }
        public string Pin { get; set; }
        public string Mensaje { get; set; }
        public string Error { get; set; }

        public RegistroUsuarioViewModel(HttpClient http)
        {
            _http = http;
        }
        public void Limpiar()
        {
            Nombre = string.Empty;
            Pin = string.Empty;
            Rol = string.Empty;
            Mensaje = string.Empty;
            Error = string.Empty;
        }
        public async Task RegistrarUsuarioAsync()
        {
            try
            {
                var usuario = new
                {
                    nombre = Nombre,
                    rol = Rol,
                    pin = int.Parse(Pin)
                };

                var respuesta = await _http.PostAsJsonAsync("api/Usuarios", usuario);

                if (respuesta.IsSuccessStatusCode)
                {
                    Mensaje = "Usuario registrado exitosamente.";
                    Error = string.Empty;
                }
                else
                {
                    var contenido = await respuesta.Content.ReadAsStringAsync();
                    Error = $"Error al registrar usuario: {contenido}";
                    Mensaje = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Error = "Error: " + ex.Message;
                Mensaje = string.Empty;
            }
        }
        public List<UsuarioDTO> Usuarios { get; set; } = new();

        public async Task ObtenerUsuariosAsync()
        {
            try
            {
                var lista = await _http.GetFromJsonAsync<List<UsuarioDTO>>("api/Usuarios");
                if (lista != null)
                {
                    Usuarios.Clear();
                    foreach (var user in lista)
                        Usuarios.Add(user);
                }
            }
            catch (Exception ex)
            {
                Error = "Error al cargar usuarios: " + ex.Message;
            }

        }

        public async Task EliminarUsuarioAsync(int id)
        {
            try
            {
                var respuesta = await _http.DeleteAsync($"api/Usuarios/{id}");
                if (respuesta.IsSuccessStatusCode)
                {
                    await ObtenerUsuariosAsync();
                }
                else
                {
                    Error = "No se pudo eliminar el usuario.";
                }
            }
            catch (Exception ex)
            {
                Error = "Error al eliminar: " + ex.Message;
            }
        }
    }
}

