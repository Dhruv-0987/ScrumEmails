using SemanticScrumEmails.Queries.DevOps;

namespace SemanticScrumEmails.WebAPI.endpoints;

public static class DevOpsEndpoints
{
    public static void MapDevOpsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("DevOps")
            .WithTags("DevOps")
            .WithOpenApi();

        group.MapGet("/SprintDetails", async (string organisation, string project, string pat, GetSprintDetailsService service) =>
            await service.GetSprintDetailsAsync(organisation, project, pat)
        );

        group.MapGet("/AssignedUserTasksForCurrentSprint", async (string organisation, string project, string pat,
            GetAssignedTasksService service) =>
            await service.GetAssignedTasksCurrentSprint(organisation, project, pat)
        );
    }
    
}