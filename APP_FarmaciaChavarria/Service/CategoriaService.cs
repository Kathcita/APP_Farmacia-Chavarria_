using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;


namespace FarmaciaChavarria.Services
{
    public class CategoriaService
    {
        private readonly HttpClient _httpClient;

        public CategoriaService()
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


        public async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            var url = "/api/Categorias";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Categoria>>();
            }
            return new List<Categoria>();
        }

        public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id)
        {
            var url = $"/api/Categorias/{id}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Categoria>();
            }
            return null;
        }


        public async Task<string> CrearCategoriaAsync(Categoria categoria)
        {
            var url = "/api/Categorias";
            var response = await _httpClient.PostAsJsonAsync(url, categoria);

            if (response.IsSuccessStatusCode)
            {
                return "Categoría creada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }


        public async Task<string> ActualizarCategoriaAsync(int id, Categoria categoria)
        {
            var url = $"/api/Categorias/{id}";
            var response = await _httpClient.PutAsJsonAsync(url, categoria);

            if (response.IsSuccessStatusCode)
            {
                return "Categoría actualizada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
        public async Task<string> EliminarCategoriaAsync(int id)
        {
            var url = $"/api/Categorias/{id}";
            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return "Categoría eliminada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
