using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;


namespace FarmaciaChavarria.Services
{
    public class ProductoService
    {
        private readonly HttpClient _httpClient;

        public ProductoService()
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

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            var response = await _httpClient.GetAsync("/api/Productos");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Producto>>();
            }
            return new List<Producto>();
        }

        // Obtener un producto por ID
        public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Productos/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Producto>();
            }
            return null;
        }

        // Crear un nuevo producto
        public async Task<string> CrearProductoAsync(Producto producto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Productos", producto);
            if (response.IsSuccessStatusCode)
            {
                return "Producto registrado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        // Actualizar un producto existente
        public async Task<string> ActualizarProductoAsync(int id, Producto producto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Productos/{id}", producto);
            if (response.IsSuccessStatusCode)
            {
                return "Producto actualizado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        // Eliminar un producto
        public async Task<string> EliminarProductoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Productos/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Producto eliminado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
