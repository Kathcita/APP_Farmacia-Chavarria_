using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;
using APP_FarmaciaChavarria.Models.PaginationModels;
using APP_FarmaciaChavarria.Models.ReporteModels;
using APP_FarmaciaChavarria.ViewModels.Reportes;

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

        public async Task<FacturaPagedResult?> ObtenerFacturasPorAñoAsync(string firstDate, string lastDate, int userId=0, int pageNumber = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/facturas-año?fechaInicio={firstDate}&fechaFin={lastDate}&userId={userId}&pageNumber={pageNumber}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FacturaPagedResult>();
            }
            return null;
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

        public async Task<List<LaboratorioVentasDTO>?> ObtenerLaboratorioMasVentasAsync(string firstDate, string lastDate, int userId = 0)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/top-laboratorios?fechaInicio={firstDate}&fechaFin={lastDate}&userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<LaboratorioVentasDTO>>();
            }
            return new List<LaboratorioVentasDTO>();
        }

        public async Task<List<CategoriaVentasDTO>?> ObtenerCategoriaMasVentasAsync(string firstDate, string lastDate, int userId = 0)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/top-categorias?fechaInicio={firstDate}&fechaFin={lastDate}&userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoriaVentasDTO>>();
            }
            return new List<CategoriaVentasDTO>();
        }

        public async Task<List<ProductoVentasDTO>?> ObtenerProductosMasVentasAsync(string firstDate, string lastDate, int userId = 0)
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/top-productos?fechaInicio={firstDate}&fechaFin={lastDate}&userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProductoVentasDTO>>();
            }
            return new List<ProductoVentasDTO>();
        }

        public async Task<DashboardData?> ObtenerDatosDashboard()
        {
            var response = await _httpClient.GetAsync($"/api/Facturas/DashboardData");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DashboardData>();
            }
            return null;
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
        public async Task<int?> GuardarFacturaAsync(Factura factura)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Facturas", factura);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return null;
            }

            var nuevaFactura = await response.Content.ReadFromJsonAsync<Factura>();
            return nuevaFactura?.id_factura;
        }


        public async Task<bool> GuardarDetalleFacturaAsync(DetalleFactura detalle)
        {
            var response = await _httpClient.PostAsJsonAsync("api/DetalleFacturas", detalle);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> GuardarFacturaYDetallesAsync(Factura factura, List<DetalleFactura> detalles)
        {
            var idFactura = await GuardarFacturaAsync(factura);
            if (idFactura == null) return false;

            foreach (var detalle in detalles)
            {
                detalle.id_factura = idFactura.Value;
                var success = await GuardarDetalleFacturaAsync(detalle);
                if (!success) return false;
            }

            return true;
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
