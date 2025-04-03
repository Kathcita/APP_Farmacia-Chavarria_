using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;

namespace FarmaciaChavarria.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("api") 
            };
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            var response = await _httpClient.GetAsync("/api/Usuarios");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Usuario>>();
            }
            return new List<Usuario>();
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Usuarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Usuario>();
            }
            return null;
        }

        public async Task<string> CrearUsuarioAsync(Usuario usuario)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Usuarios", usuario);
            if (response.IsSuccessStatusCode)
            {
                return "Usuario registrado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarUsuarioAsync(int id, Usuario usuario)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Usuarios/{id}", usuario);
            if (response.IsSuccessStatusCode)
            {
                return "Usuario actualizado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> EliminarUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Usuarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Usuario eliminado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
