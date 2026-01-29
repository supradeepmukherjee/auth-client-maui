using Microsoft.Extensions.Logging;
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
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<AuthHttpHandler> _logger;

        public AuthHttpHandler(AuthLocalService local, IHttpClientFactory httpFactory, ILogger<AuthHttpHandler> logger)
        {
            _local = local;
            _httpFactory = httpFactory;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var (access, refresh) = await _local.GetTokenAsync(requireBiometric: false);
            if (!string.IsNullOrEmpty(access))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);
            }

            var response = await base.SendAsync(request, ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshToken = refresh ?? await _local.GetRefreshTokenSilentlyAsync();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        var client = _httpFactory.CreateClient("noauth");
                        var res = await client.PostAsJsonAsync("/refresh", new { RefreshToken = refreshToken }, ct);
                        if (res.IsSuccessStatusCode)
                        {
                            var token = await res.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
                            if (token != null)
                            {
                                await _local.SaveTokensAsync(token.AccessToken, token.RefreshToken);
                                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                                response.Dispose();
                                response = await base.SendAsync(request, ct);
                                return response;
                            }
                        }
                        else
                        {
                            await _local.ClearTokensAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Silent refresh failed");
                        await _local.ClearTokensAsync();
                    }
                }
                else
                {
                    await _local.ClearTokensAsync();
                }
            }

            return response;
        }
    }
}
