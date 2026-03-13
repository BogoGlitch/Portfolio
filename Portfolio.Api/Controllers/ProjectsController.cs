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

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="createProjectDto">The project data used to create the project.</param>
    /// <returns>The created project.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProjectReadDto>> CreateProject(CreateProjectDto createProjectDto)
    {
        var createdProject = await _projectService.CreateProjectAsync(createProjectDto);

        return CreatedAtAction(
            nameof(GetProjectBySlug),
            new { slug = createdProject.Slug },
            createdProject);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="updateProjectDto">The updated project data.</param>
    /// <returns>The updated project if found.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProjectReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectReadDto>> UpdateProject(int id, UpdateProjectDto updateProjectDto)
    {
        var updatedProject = await _projectService.UpdateProjectAsync(id, updateProjectDto);

        if (updatedProject is null)
        {
            return NotFound();
        }
        return Ok(updatedProject);
    }

    /// <summary>
    /// Deletes a project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <returns>No content if the project was deleted; otherwise not found.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var projectToDelete = await _projectService.DeleteProjectAsync(id);

        if (!projectToDelete)
        {
            return NotFound();
        }
        return NoContent();
    }

}
