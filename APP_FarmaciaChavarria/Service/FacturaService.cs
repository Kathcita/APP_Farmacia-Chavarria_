using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.ReporteModels;

namespace FarmaciaChavarria.Services
{
    public class FacturaService
    {
        private readonly HttpClient _httpClient;

        public FacturaService(HttpClient httpClient)
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
       
        public async Task<List<Factura>?> ObtenerFacturasAsync()
        {
            var response = await _httpClient.GetAsync("/api/Facturas");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Factura>>();
            }
            return new List<Factura>();
        }

        public async Task<List<Factura>?> ObtenerFacturasPorAñoAsync(string firstDate, string lastDate, int userId=0)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/facturas-año?fechaInicio={firstDate}&fechaFin={lastDate}&&userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Factura>>();
            }
            return new List<Factura>();
        }

        public async Task<List<RevenueDataItem>?> ObtenerFacturasReporteAsync(string firstDate, string lastDate, int userId = 0)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/ventas-por-mes-año?fechaInicio={firstDate}&fechaFin={lastDate}&userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<RevenueDataItem>>();
            }
            return new List<RevenueDataItem>();
        }

        public async Task<Factura?> ObtenerFacturaPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Factura>();
            }
            return null;
        }

        public async Task<string> CrearFacturaAsync(Factura factura, List<DetalleFactura> detalles)
        {
            var facturaConDetalles = new
            {
                Factura = factura,
                Detalles = detalles
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Facturas", facturaConDetalles);
            if (response.IsSuccessStatusCode)
            {
                return "Factura creada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarFacturaAsync(int id, Factura factura)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Facturas/{id}", factura);
            if (response.IsSuccessStatusCode)
            {
                return "Factura actualizada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> EliminarFacturaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Facturas/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Factura eliminada exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
