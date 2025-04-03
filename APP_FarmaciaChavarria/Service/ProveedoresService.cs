using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;

namespace FarmaciaChavarria.Services
{
    public class ProveedorService
    {
        private readonly HttpClient _httpClient;

        public ProveedorService()
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

        public async Task<List<Proveedor>> ObtenerProveedoresAsync()
        {
            var response = await _httpClient.GetAsync("/api/Proveedors");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Proveedor>>();
            }
            return new List<Proveedor>();
        }

        public async Task<Proveedor?> ObtenerProveedorPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Proveedors/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Proveedor>();
            }
            return null;
        }

        public async Task<string> CrearProveedorAsync(Proveedor proveedor)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Proveedors", proveedor);
            if (response.IsSuccessStatusCode)
            {
                return "Proveedor registrado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarProveedorAsync(int id, Proveedor proveedor)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Proveedors/{id}", proveedor);
            if (response.IsSuccessStatusCode)
            {
                return "Proveedor actualizado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> EliminarProveedorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Proveedors/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Proveedor eliminado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
