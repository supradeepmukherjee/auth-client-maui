using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace client_maui.Services
{
    public class AuthHttpHandler:DelegatingHandler
    {
        private readonly AuthLocalService _local;
        private readonly HttpClient _refreshClient;
        

        public AuthHttpHandler(AuthLocalService local)
        {
            _local = local;
            _refreshClient = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:5166")
            };
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var (access, refresh) = await _local.GetTokenAsync(requireBiometric: false);
            if (!string.IsNullOrEmpty(access)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);

            var response = await base.SendAsync(request, ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized && refresh != null)
            {
                var refreshResponse = await _refreshClient.PostAsJsonAsync(
                    "/refresh",
                    new { RefreshToken = refresh },
                    ct);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var newToken =
                        await refreshResponse.Content.ReadFromJsonAsync<TokenResponse>(ct);

                    if (newToken != null)
                    {
                        await _local.SaveTokensAsync(
                            newToken.AccessToken,
                            newToken.RefreshToken);

                        request.Headers.Authorization =
                            new AuthenticationHeaderValue("Bearer", newToken.AccessToken);

                        response = await base.SendAsync(request, ct);
                    }
                }
            }

            return response;
        }
    }
}
