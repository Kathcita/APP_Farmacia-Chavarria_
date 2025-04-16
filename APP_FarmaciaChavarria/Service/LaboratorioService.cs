using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API_FarmaciaChavarria.Models;


namespace FarmaciaChavarria.Services
{
    public class LaboratorioService
    {
        private readonly HttpClient _httpClient;

        public LaboratorioService(HttpClient httpClient)
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

        public async Task<List<Laboratorio>> ObtenerLaboratoriosAsync()
        {
            var response = await _httpClient.GetAsync("/api/Laboratorios");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Laboratorio>>();
            }
            return new List<Laboratorio>();
        }


        public async Task<Laboratorio?> ObtenerLaboratorioPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Laboratorios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Laboratorio>();
            }
            return null;
        }

        public async Task<List<Laboratorio?>> ObtenerLaboratorioPorNombreAsync(string nombre)
        {
            var url = $"/api/Laboratorios/nombre/{nombre}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Laboratorio>>();
            }
            return null;
        }


        public async Task<string> CrearLaboratorioAsync(Laboratorio laboratorio)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Laboratorios", laboratorio);
            if (response.IsSuccessStatusCode)
            {
                return "Laboratorio creado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> ActualizarLaboratorioAsync(int id, Laboratorio laboratorio)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Laboratorios/{id}", laboratorio);
            if (response.IsSuccessStatusCode)
            {
                return "Laboratorio actualizado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }

        public async Task<string> EliminarLaboratorioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Laboratorios/{id}");
            if (response.IsSuccessStatusCode)
            {
                return "Laboratorio eliminado exitosamente.";
            }
            return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
    }
}
