using SemanticScrumEmails.interfaces;

namespace SemanticScrumEmails.Queries.DevOps;

public class GetAssignedTasksService(IDevOpsService devOpsService)
{
   public async Task<IResult> GetAssignedTasksCurrentSprint(string organisation, string project, string pat)
   {
      if (string.IsNullOrEmpty(organisation) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(pat))
      {
         return Results.BadRequest("Organization, Project, and Personal Access Token are required.");
      }

      var iterationPath = await GetCurrentIterationPath(organisation, project, pat);

      try
      {
         var assignedTasks = await devOpsService.GetMyAssignedPBIsAsync(organisation, project, pat, iterationPath);
         return assignedTasks != null ? Results.Ok(assignedTasks) : Results.NotFound("Assigned Tasks Not Found");
      }
      catch
      {
         return Results.Problem("An error occurred while retrieving Assigned tasks.");
      }
   }

   private async Task<string> GetCurrentIterationPath(string org, string project, string pat)
   {
      var sprintData = await devOpsService.GetCurrentSprintDataAsync(org, project, pat);

      if (sprintData.HasValue && sprintData.Value.TryGetProperty("path", out var pathElement))
      {
         return pathElement.GetString();
      }
      
      return String.Empty;
   }
}