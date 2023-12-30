using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SemanticScrumEmails.interfaces;
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

        group.MapGet("/GetAssignedUserPBIs", async (string organisation, string project, string pat, string iteration,
            GetAssignedTasksService service) =>
            await service.GetAssignedTasksCurrentSprint(organisation, project, pat, iteration)
        );
    }
    
}