using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Features.Technologies.Commands.CreateTechnology;
using Portfolio.Api.Features.Technologies.Commands.DeleteTechnology;
using Portfolio.Api.Features.Technologies.Commands.UpdateTechnology;
using Portfolio.Api.Features.Technologies.Queries.GetTechnologies;
using Portfolio.Api.Features.Technologies.Queries.GetTechnologyBySlug;
using Portfolio.Api.Filters;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TechnologiesController : ControllerBase
{
    private readonly GetTechnologiesQueryHandler _getTechnologies;
    private readonly GetTechnologyBySlugQueryHandler _getTechnologyBySlug;
    private readonly CreateTechnologyCommandHandler _createTechnology;
    private readonly UpdateTechnologyCommandHandler _updateTechnology;
    private readonly DeleteTechnologyCommandHandler _deleteTechnology;

    public TechnologiesController(
        GetTechnologiesQueryHandler getTechnologies,
        GetTechnologyBySlugQueryHandler getTechnologyBySlug,
        CreateTechnologyCommandHandler createTechnology,
        UpdateTechnologyCommandHandler updateTechnology,
        DeleteTechnologyCommandHandler deleteTechnology)
    {
        _getTechnologies = getTechnologies;
        _getTechnologyBySlug = getTechnologyBySlug;
        _createTechnology = createTechnology;
        _updateTechnology = updateTechnology;
        _deleteTechnology = deleteTechnology;
    }

    /// <summary>
    /// Retrieves all technologies.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TechnologyReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TechnologyReadDto>>> GetTechnologies()
    {
        var result = await _getTechnologies.HandleAsync(new GetTechnologiesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a technology by its slug.
    /// </summary>
    [HttpGet("{slug:minlength(1)}")]
    [ProducesResponseType(typeof(TechnologyReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TechnologyReadDto>> GetTechnologyBySlug(string slug)
    {
        var result = await _getTechnologyBySlug.HandleAsync(new GetTechnologyBySlugQuery(slug));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new technology.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateTechnologyDto>))]
    [ProducesResponseType(typeof(TechnologyReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TechnologyReadDto>> CreateTechnology([FromBody] CreateTechnologyDto createTechnologyDto)
    {
        var result = await _createTechnology.HandleAsync(new CreateTechnologyCommand(createTechnologyDto));
        return CreatedAtAction(nameof(GetTechnologyBySlug), new { slug = result.Slug }, result);
    }

    /// <summary>
    /// Updates an existing technology.
    /// </summary>
    [Authorize]
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateTechnologyDto>))]
    [ProducesResponseType(typeof(TechnologyReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TechnologyReadDto>> UpdateTechnology(int id, [FromBody] UpdateTechnologyDto updateTechnologyDto)
    {
        var result = await _updateTechnology.HandleAsync(new UpdateTechnologyCommand(id, updateTechnologyDto));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a technology by its unique identifier.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTechnology(int id)
    {
        var deleted = await _deleteTechnology.HandleAsync(new DeleteTechnologyCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}
