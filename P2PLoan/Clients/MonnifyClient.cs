using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace P2PLoan.Clients;

public class MonnifyClient
{
    private readonly IConfiguration configuration;

    public HttpClient Client { get; }
    public MonnifyClient(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public MonnifyClient(HttpClient client)
    {
        Client = client;
        Client.BaseAddress = new Uri(configuration["Monnify:BaseUrl"]);
        Client.Timeout = new TimeSpan(0, 0, 30);
        Client.DefaultRequestHeaders.Clear();
    }
}

public class TokenHandler : DelegatingHandler
{
    private readonly string _tokenUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string _accessToken;
    private DateTime _tokenExpiration;
    private readonly HttpClient _authClient;
    private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

    public TokenHandler(string tokenUrl, string clientId, string clientSecret)
    {
        _tokenUrl = tokenUrl;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _authClient = new HttpClient();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
        {
            await FetchTokenAsync();
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        // If the token has expired, fetch a new one and retry the request
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await FetchTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task FetchTokenAsync()
    {
        await _tokenSemaphore.WaitAsync();
        try
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
            {
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                var request = new HttpRequestMessage(HttpMethod.Post, _tokenUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                var response = await _authClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<MonnifyApiResponse<TokenResponseBody>>(content);

                _accessToken = tokenResponse.ResponseBody.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ResponseBody.ExpiresIn - 60); // Subtract 60 seconds for buffer
            }
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }


    private class TokenResponseBody
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; } // Expiry time in seconds
    }


}