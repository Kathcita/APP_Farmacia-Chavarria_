using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Service
{
    public class TokenHandlerService
    {
        private readonly HttpClient _httpClient;

        public TokenHandlerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
