using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using APP_FarmaciaChavarria.Models.AuthRequest;
using Newtonsoft.Json;

namespace FarmaciaChavarria.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginUser(LoginRequest loginRequest)
        {
            try
            {
                var url = "api/login"; 
                var response = await _httpClient.PostAsJsonAsync(url, loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<LoginResponse>(responseData);
                }
                else
                {
                    throw new Exception("Credenciales incorrectas");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
