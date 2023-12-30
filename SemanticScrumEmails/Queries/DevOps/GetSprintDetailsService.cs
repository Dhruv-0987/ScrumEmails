using SemanticScrumEmails.interfaces;

namespace SemanticScrumEmails.Queries.DevOps;

public class GetSprintDetailsService(IDevOpsService devOpsService)
{
    public async Task<IResult> GetSprintDetailsAsync(string organisation, string project, string pat)
    {
        if (string.IsNullOrEmpty(organisation) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(pat))
        {
            return Results.BadRequest("Organization, Project, and Personal Access Token are required.");
        }

        try
        {
            var sprintData = await devOpsService.GetSprintDataAsync(organisation, project, pat);
            return sprintData != null ? Results.Ok(sprintData) : Results.NotFound("An error occurred while retrieving sprint data.");
        }
        catch (Exception ex)
        {
            // Log the exception here if needed
            return Results.Problem("An error occurred while retrieving sprint data.");
        }
    }

}