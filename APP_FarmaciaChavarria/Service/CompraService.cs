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

        public CompraService()
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


        public async Task<List<Compra>> ObtenerComprasAsync()
        {
            var response = await _httpClient.GetAsync("/api/Compras");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Compra>>();
            }
            return new List<Compra>();
        }

        public async Task<Compra?> ObtenerCompraPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Compras/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Compra>();
            }
            return null;
        }

        public async Task<string> CrearCompraAsync(Compra compra, List<DetalleCompra> detalles)
        {
            var compraConDetalles = new
            {
                Compra = compra,
                Detalles = detalles
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Compras", compraConDetalles);
            if (response.IsSuccessStatusCode)
            {
                return "Compra creada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarCompraAsync(int id, Compra compra)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Compras/{id}", compra);
            if (response.IsSuccessStatusCode)
            {
                return "Compra actualizada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> EliminarCompraAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Compras/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Compra eliminada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
