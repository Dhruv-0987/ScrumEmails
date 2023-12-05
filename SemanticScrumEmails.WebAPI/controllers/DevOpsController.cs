using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.VisualBasic.CompilerServices;
using SemanticScrumEmails.interfaces;
using SemanticScrumEmails.WebAPI.DTOs;

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
    
    [HttpGet("SprintDetails")]
    public async Task<ActionResult<JsonElement>> GetSprintDetails([FromQuery] string organisation, [FromQuery] string project, 
        [FromQuery] string pat)
    {
        if (string.IsNullOrEmpty(organisation) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(pat))
        {
            return BadRequest("Organization, Project, and Personal Access Token are required.");
        }

        try
        {
            var sprintData = await _devOpsService.GetSprintDataAsync(organisation, project, pat);
            // ReSharper disable once HeapView.BoxingAllocation
            return Ok(sprintData); // This will return JSON response
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving sprint data.");
        }
    }

    [HttpGet("GetAssignedPBIs")]
    public async Task<ActionResult<JsonElement?>> GetAssignedPBIs([FromQuery] string organisation,
        [FromQuery] string project,
        [FromQuery] string pat,
        [FromQuery] string iteration)
    {
        if (string.IsNullOrEmpty(organisation) || string.IsNullOrEmpty(project) || string.IsNullOrEmpty(pat))
        {
            return BadRequest("Organization, Project, and Personal Access Token are required.");
        }

        try
        {
            var assignedPBIs = await _devOpsService.GetMyAssignedPBIsAsync(organisation, project, pat, iteration);
            // ReSharper disable once HeapView.BoxingAllocation
            return Ok(assignedPBIs); // This will return JSON response
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving sprint data.");
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