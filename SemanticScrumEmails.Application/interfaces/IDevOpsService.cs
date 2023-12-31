using System.Text.Json;

namespace SemanticScrumEmails.interfaces;

public interface IDevOpsService
{
    public Task<JsonElement> GetAllProjectsAsync(string organization, string personalAccessToken);
    public Task<JsonElement?> GetCurrentSprintDataAsync(string organization, string project, string personalAccessToken);
    public Task<JsonElement?> GetMyAssignedPBIsAsync(string organization, string project, string personalAccessToken, string iterationPath);
    public Task<JsonElement?> GetWorkItemDetailsAsync(int workItemId, string personalAccessToken, string organization);
    public Task<JsonElement?> GetCurrentIterationAsync(string organization, string project, string team,
        string personalAccessToken);
}