using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ModelsDTO;
using APP_FarmaciaChavarria.Models.PaginationModels;


namespace FarmaciaChavarria.Services
{
    public class ProductoService
    {
        private readonly HttpClient _httpClient;

        public ProductoService(HttpClient httpClient)
        {
            /*var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("api") 
            };*/

            _httpClient = httpClient;
        }

        public async Task<ProductoPagedResult?> ObtenerProductosAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"/api/Productos?pageNumber={pageNumber}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductoPagedResult>();
            }

            return null; // O manejar error de otra forma
        }

        public async Task<ProductoPagedResult?> ObtenerProductosEscasosAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"/api/Productos/medicamentos-escasos?pageNumber={pageNumber}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductoPagedResult>();
            }

            return null; // O manejar error de otra forma
        }


        // Obtener un producto por ID
        public async Task<ProductoDTO?> ObtenerProductoPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Productos/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductoDTO>();
            }
            return null;
        }

        public async Task<ProductoPagedResult?> ObtenerProductoPorNombreAsync(string nombre, int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"/api/Productos/nombre/{nombre}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductoPagedResult>();
            }
            return null;
        }

        public async Task<ProductoPagedResult?> ObtenerProductoPorCategoriaAsync(int id, int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"/api/Productos/categoria/{id}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductoPagedResult>();
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
