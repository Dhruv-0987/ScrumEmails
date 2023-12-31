using System.Net.Http.Headers;
using System.Text;

namespace SemanticScrumEmails.WebAPI.HttpService;

public class HttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService(IHttpClientFactory httpClientFactory, string baseUrl, string pat)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));
    }

    public async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await _httpClient.GetAsync(uri);
    }
}