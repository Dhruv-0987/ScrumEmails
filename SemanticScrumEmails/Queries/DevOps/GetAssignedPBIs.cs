using SemanticScrumEmails.interfaces;

namespace SemanticScrumEmails.Queries.DevOps;

public class GetAssignedTasksService(IDevOpsService devOpsService)
{
   public async Task<IResult> GetAssignedTasksCurrentSprint(string organisation, string project, string pat, string iteration)
   {
      if (string.IsNullOrEmpty(organisation) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(pat) || string.IsNullOrEmpty(iteration))
      {
         return Results.BadRequest("Organization, Project, and Personal Access Token are required.");
      }

      try
      {
         var assignedTasks = await devOpsService.GetMyAssignedPBIsAsync(organisation, project, pat, iteration);
         return assignedTasks != null ? Results.Ok(assignedTasks) : Results.Problem("Assigned Tasks Not Found");
      }
      catch
      {
         return Results.Problem("An error occurred while retrieving Assigned tasks.");
      }
   }
}