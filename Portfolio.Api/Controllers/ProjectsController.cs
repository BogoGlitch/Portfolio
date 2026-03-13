using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Interfaces;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>A collection of projects.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectReadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetProjects()
    {
        var projects = await _projectService.GetProjectsAsync();
        return Ok(projects);
    }

    /// <summary>
    /// Retrieves a project by its slug.
    /// </summary>
    /// <param name="slug">The unique slug for the project.</param>
    /// <returns>The matching project if found.</returns>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ProjectReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectReadDto>> GetProjectBySlug(string slug)
    {

        if (string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest();
        }

        var project = await _projectService.GetProjectBySlugAsync(slug);
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectReadDto>> CreateProject(CreateProjectDto createProjectDto)
    {
        var createdProject = await _projectService.CreateProjectAsync(createProjectDto);

        return CreatedAtAction(
            nameof(GetProjectBySlug),
            new { slug = createdProject.Slug },
            createdProject);
    }
}
