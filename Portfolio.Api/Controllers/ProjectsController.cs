using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Features.Projects.Commands.CreateProject;
using Portfolio.Api.Features.Projects.Commands.DeleteProject;
using Portfolio.Api.Features.Projects.Commands.UpdateProject;
using Portfolio.Api.Features.Projects.Queries.GetProjectBySlug;
using Portfolio.Api.Features.Projects.Queries.GetProjects;
using Portfolio.Api.Filters;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly GetProjectsQueryHandler _getProjects;
    private readonly GetProjectBySlugQueryHandler _getProjectBySlug;
    private readonly CreateProjectCommandHandler _createProject;
    private readonly UpdateProjectCommandHandler _updateProject;
    private readonly DeleteProjectCommandHandler _deleteProject;

    public ProjectsController(
        GetProjectsQueryHandler getProjects,
        GetProjectBySlugQueryHandler getProjectBySlug,
        CreateProjectCommandHandler createProject,
        UpdateProjectCommandHandler updateProject,
        DeleteProjectCommandHandler deleteProject)
    {
        _getProjects = getProjects;
        _getProjectBySlug = getProjectBySlug;
        _createProject = createProject;
        _updateProject = updateProject;
        _deleteProject = deleteProject;
    }

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetProjects([FromQuery] ProjectQueryParametersDto queryParameters)
    {
        var result = await _getProjects.HandleAsync(new GetProjectsQuery(queryParameters));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a project by its slug.
    /// </summary>
    [HttpGet("{slug:minlength(1)}")]
    [ProducesResponseType(typeof(ProjectReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProjectReadDto>> GetProjectBySlug(string slug)
    {
        var result = await _getProjectBySlug.HandleAsync(new GetProjectBySlugQuery(slug));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateProjectDto>))]
    [ProducesResponseType(typeof(ProjectReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProjectReadDto>> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        var result = await _createProject.HandleAsync(new CreateProjectCommand(createProjectDto));
        return CreatedAtAction(nameof(GetProjectBySlug), new { slug = result.Slug }, result);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    [Authorize]
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateProjectDto>))]
    [ProducesResponseType(typeof(ProjectReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProjectReadDto>> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        var result = await _updateProject.HandleAsync(new UpdateProjectCommand(id, updateProjectDto));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a project by its unique identifier.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var deleted = await _deleteProject.HandleAsync(new DeleteProjectCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}
