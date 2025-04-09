using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;


namespace FarmaciaChavarria.Services
{
    public class ProductoCaducarService
    {
        private readonly HttpClient _httpClient;

        public ProductoCaducarService()
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

        public async Task<List<ProductoCaducar>> ObtenerProductosCaducarAsync()
        {
            var response = await _httpClient.GetAsync("/api/ProductosCaducar");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductoCaducar>>();
            }
            return new List<ProductoCaducar>();
        }


        public async Task<ProductoCaducar?> ObtenerProductoCaducarPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/ProductosCaducar/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductoCaducar>();
            }
            return null;
        }


        public async Task<string> CrearProductoCaducarAsync(ProductoCaducar productoCaducar)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ProductosCaducar", productoCaducar);
            if (response.IsSuccessStatusCode)
            {
                return "Producto a caducar registrado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarProductoCaducarAsync(int id, ProductoCaducar productoCaducar)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/ProductosCaducar/{id}", productoCaducar);
            if (response.IsSuccessStatusCode)
            {
                return "Producto a caducar actualizado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> EliminarProductoCaducarAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/ProductosCaducar/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Producto a caducar eliminado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
