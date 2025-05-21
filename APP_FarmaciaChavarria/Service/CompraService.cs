using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;

namespace FarmaciaChavarria.Services
{
    public class CompraService
    {
        private readonly HttpClient _httpClient;

        public CompraService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Compra>?> ObtenerComprasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Compras");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Compra>>() ?? new List<Compra>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener compras: {ex.Message}");
            }
            return new List<Compra>();
        }

        public async Task<Compra?> ObtenerCompraPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Compras/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Compra>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la compra con ID {id}: {ex.Message}");
            }
            return null;
        }

        public async Task<int?> CrearCompraAsync(Compra compra)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Compras", compra);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var nuevaCompra = await response.Content.ReadFromJsonAsync<Compra>();
                return nuevaCompra?.id_compra;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> GuardarDetalleCompraAsync(DetalleCompra detalle)
        {
            var response = await _httpClient.PostAsJsonAsync("api/DetalleCompras", detalle);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> ActualizarCompraAsync(int id, Compra compra)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/Compras/{id}", compra);
                if (response.IsSuccessStatusCode)
                {
                    return "Compra actualizada exitosamente.";
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode} - {response.ReasonPhrase}. Detalle: {errorContent}";
            }
            catch (Exception ex)
            {
                return $"Excepción al actualizar compra: {ex.Message}";
            }
        }

        public async Task<string> EliminarCompraAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Compras/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return "Compra eliminada exitosamente.";
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode} - {response.ReasonPhrase}. Detalle: {errorContent}";
            }
            catch (Exception ex)
            {
                return $"Excepción al eliminar compra: {ex.Message}";
            }
        }
    }
}
