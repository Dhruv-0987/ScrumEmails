using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SemanticScrumEmails.constants;
using SemanticScrumEmails.interfaces;

namespace SemanticScrumEmails.services;

public class DevOpsService: IDevOpsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DevOpsService> _logger;
    public DevOpsService(IHttpClientFactory httpClientFactory, ILogger<DevOpsService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }
    
    public async Task<JsonElement> GetAllProjectsAsync(string organization, string personalAccessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));
        
        var url = $"https://dev.azure.com/{organization}/_apis/projects?api-version=6.0";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content).RootElement;
    }
    
    public async Task<JsonElement?> GetCurrentSprintDataAsync(string organization, string project, string personalAccessToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));

            var url =
                $"{DevOpsConstants.BaseUrl}{organization}/{project}/{DevOpsConstants.IterationsEndpoint}?api-version={DevOpsConstants.ApiVersion}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("request failed");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var iterations = JsonDocument.Parse(content).RootElement;

            var currentDate = DateTime.Now;
            foreach (var iteration in iterations.GetProperty("value").EnumerateArray().Reverse())
            {
                var attributes = iteration.GetProperty("attributes");
             
                if (!attributes.TryGetProperty("startDate", out var startDateElement) || startDateElement.ValueKind == JsonValueKind.Null ||
                    !attributes.TryGetProperty("finishDate", out var finishDateElement) || finishDateElement.ValueKind == JsonValueKind.Null)
                {
                    continue; // Skip this iteration if dates are missing or null
                }

                var startDate = DateTime.Parse(startDateElement.GetString());
                var finishDate = DateTime.Parse(finishDateElement.GetString());

                if (currentDate >= startDate && currentDate <= finishDate)
                {
                    return iteration; // Current iteration found
                }
            }
            
            _logger.LogError("No current iteration found");
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<JsonElement?> GetMyAssignedPBIsAsync(string organization, string project, string personalAccessToken, string iterationPath)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));

            var wiqlQuery = $@"
                            SELECT [System.Id], [System.Title], [System.State] 
                            FROM WorkItems 
                            WHERE [System.WorkItemType] = 'Product Backlog Item' 
                            AND [System.AssignedTo] = @me 
                            AND [System.IterationPath] = '{iterationPath}'
                            ORDER BY [System.CreatedDate] DESC";

            var requestBody = new
            {
                query = wiqlQuery
            };

            var url = $"{DevOpsConstants.BaseUrl}{organization}/{project}/{DevOpsConstants.wiqlEndpoint}?api-version={DevOpsConstants.ApiVersion}";
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("request failed");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content).RootElement;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assigned PBIs for current sprint");
            return null;
        }
    }
    
    public async Task<JsonElement?> GetWorkItemDetailsAsync(int workItemId, string personalAccessToken, string organization)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));

            var url = $"{DevOpsConstants.BaseUrl}{organization}/{DevOpsConstants.workItemEndpoint}/{workItemId}?api-version={DevOpsConstants.ApiVersion}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch work item details");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content).RootElement;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching work item details");
            return null;
        }
    }
    
    public async Task<JsonElement?> GetCurrentIterationAsync(string organization, string project, string team, string personalAccessToken)
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));

            var url = $"https://dev.azure.com/{organization}/{project}/{team}/_apis/work/teamsettings/iterations?api-version=6.0&$timeframe=current";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                // Handle error or return null
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content).RootElement;
        }
        catch (Exception ex)
        {
            // Handle exception
            return null;
        }
    }


}