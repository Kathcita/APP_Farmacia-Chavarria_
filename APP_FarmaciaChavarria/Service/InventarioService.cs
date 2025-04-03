using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;

namespace FarmaciaChavarria.Services
{
    public class InventarioService
    {
        private readonly HttpClient _httpClient;

        public InventarioService()
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

        public async Task<List<Inventario>> ObtenerInventariosAsync()
        {
            var response = await _httpClient.GetAsync("/api/Inventarios");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Inventario>>();
            }
            return new List<Inventario>();
        }

   
        public async Task<Inventario?> ObtenerInventarioPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Inventarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Inventario>();
            }
            return null;
        }

        
        public async Task<string> CrearOActualizarInventarioAsync(Inventario inventario, List<Inventario> movimientos)
        {
            var inventarioConMovimientos = new
            {
                Inventario = inventario,
                Movimientos = movimientos
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Inventarios", inventarioConMovimientos);
            if (response.IsSuccessStatusCode)
            {
                return "Inventario y movimientos creados/actualizados exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarInventarioAsync(int id, Inventario inventario)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Inventarios/{id}", inventario);
            if (response.IsSuccessStatusCode)
            {
                return "Inventario actualizado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

   
        public async Task<string> EliminarInventarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Inventarios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Inventario eliminado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
