using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SemanticScrumEmails.interfaces;

namespace SemanticScrumEmails.WebAPI.controllers;

[Route("[controller]")]
[ApiController]
public class DevOpsController : ControllerBase
{
    private readonly IDevOpsService _devOpsService;
    
    public DevOpsController(IDevOpsService devOpsService)
    {
        _devOpsService = devOpsService;
    }
    
    [HttpGet("GetProjects")]
    public async Task<ActionResult<JsonElement>> GetProjects(string organization, string pat)
    {
        try
        {
            var projects = await _devOpsService.GetAllProjectsAsync(organization, pat);
            // ReSharper disable once HeapView.BoxingAllocation
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving projects.");
        }
    }
    
    [HttpGet("GetWorkItemDetails")]
    public async Task<ActionResult<JsonElement?>> GetWorkItemDetails([FromQuery] int itemId, [FromQuery] string pat,
        [FromQuery] string organisation)
    {
        if (string.IsNullOrEmpty(organisation) || itemId <= 0 || string.IsNullOrEmpty(pat))
        {
            return BadRequest("Organization, Project, and Personal Access Token are required.");
        }

        try
        {
            var workItemDetail = await _devOpsService.GetWorkItemDetailsAsync(itemId, pat, organisation);
            // ReSharper disable once HeapView.BoxingAllocation
            return Ok(workItemDetail); // This will return JSON response
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving sprint data.");
        }
    }

    [HttpGet("GetCurrentIteration")]
    public async Task<IActionResult> GetCurrentSprint([FromQuery] string organization, [FromQuery] string project,
        [FromQuery] string team,
        [FromQuery] string pat)
    {
        if (string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(pat) || string.IsNullOrEmpty(team))
        {
            return BadRequest("Organization, Project, and Personal Access Token are required.");
        }

        try
        {
            var currentSprintDetail = await _devOpsService.GetCurrentIterationAsync(organization, project, team, pat);
            // ReSharper disable once HeapView.BoxingAllocation
            return Ok(currentSprintDetail);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving sprint data.");
        }
    }
}