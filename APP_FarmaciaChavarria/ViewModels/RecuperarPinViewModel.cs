using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class RecuperarPinViewModel
{
    public string Usuario { get; set; }
    public string Email { get; set; }
    public string Mensaje { get; set; }
    public string Error { get; set; }

    private readonly HttpClient _http;

    public RecuperarPinViewModel(HttpClient http)
    {
        _http = http;
    }

    public async Task RecuperarAsync()
    {
        try
        {
            var respuesta = await _http.PostAsJsonAsync("api/RecuperarPin", new { Usuario, Email });

            if (respuesta.IsSuccessStatusCode)
            {
                var data = await respuesta.Content.ReadFromJsonAsync<Respuesta>();
                Mensaje = data?.mensaje;
                Error = null;
            }
            else
            {
                var error = await respuesta.Content.ReadFromJsonAsync<Respuesta>();
                Error = error?.mensaje ?? "Error al recuperar PIN.";
                Mensaje = null;
            }
        }
        catch (Exception ex)
        {
            Error = "Error de red: " + ex.Message;
            Mensaje = null;
        }
    }

    private class Respuesta
    {
        public string mensaje { get; set; }
    }
}
