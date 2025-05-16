using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using APP_FarmaciaChavarria.Models.AuthRequest;
using APP_FarmaciaChavarria.Models;
using Newtonsoft.Json;
using API_FarmaciaChavarria.Models;

namespace FarmaciaChavarria.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public string Token { get; private set; }

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginUser(LoginRequest loginRequest)
        {
            try
            {
                var url = "api/login"; 
                var response = await _httpClient.PostAsJsonAsync(url, loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tokenObj = JsonConvert.DeserializeObject<TokenResponse>(json);
                    Token = tokenObj.Token;

                    // Ejemplo: agregar el token a cada petición automáticamente
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

                    return Token;
                }
                else
                {
                    return string.Empty;
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
