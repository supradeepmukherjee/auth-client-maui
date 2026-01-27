using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace client_maui.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<TokenResponse?> LoginAsync(string identifier, string password)
        {
            var dto = new
            {
                Identifier = identifier,
                Password = password
            };
            var res = await _http.PostAsJsonAsync("/login", dto);
            if (!res.IsSuccessStatusCode) return null;
            var token = await res.Content.ReadFromJsonAsync<TokenResponse>();
            return token;
        }

        public async Task<bool> RegisterAsync(string email, string phone, string password, string displayName)
        {
            var dto = new
            {
                Email = email,
                PhoneNumber = phone,
                Password = password,
                DisplayName = displayName
            };
            var res = await _http.PostAsJsonAsync("/register", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<TokenResponse?> RefreshAsync(string refreshToken)
        {
            var res = await _http.PostAsJsonAsync("/refresh", new { RefreshToken = refreshToken });
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<TokenResponse>();
        }

        public async Task<TokenResponse?> ExternalLoginAsync(string provider, string idToken)
        {
            var res = await _http.PostAsJsonAsync("/external-login", new
            {
                Provider = provider,
                IdToken = idToken
            });
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<TokenResponse>();
        }

        public async Task<string?> GetMeAsync(string accessToken)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "/me");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadAsStringAsync();
        }
    }
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresInSeconds { get; set; }
    }
}
