using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Filters;
using Portfolio.Api.Interfaces;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TechnologiesController : ControllerBase
{
    private readonly ITechnologyService _technologyService;

    public TechnologiesController(ITechnologyService technologyService)
    {
        _technologyService = technologyService;
    }

    /// <summary>
    /// Retrieves all technologies.
    /// </summary>
    /// <returns>A list of technologies.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TechnologyReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TechnologyReadDto>>> GetTechnologies()
    {
        var technologies = await _technologyService.GetTechnologiesAsync();
        return Ok(technologies);
    }

    /// <summary>
    /// Retrieves a technology by its slug.
    /// </summary>
    /// <param name="slug">The unique slug for the technology.</param>
    /// <returns>The matching technology if found.</returns>
    [HttpGet("{slug:minlength(1)}")]
    [ProducesResponseType(typeof(TechnologyReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TechnologyReadDto>> GetTechnologyBySlug(string slug)
    {
        var technology = await _technologyService.GetTechnologyBySlugAsync(slug);
        if (technology == null)
        {
            return NotFound();
        }
        return Ok(technology);
    }

    /// <summary>
    /// Creates a new technology.
    /// </summary>
    /// <param name="createTechnologyDto">The technology data used to create the technology.</param>
    /// <returns>The created technology.</returns>
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateTechnologyDto>))]
    [ProducesResponseType(typeof(TechnologyReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TechnologyReadDto>> CreateTechnology([FromBody] CreateTechnologyDto createTechnologyDto)
    {
        var createdTechnology = await _technologyService.CreateTechnologyAsync(createTechnologyDto);

        return CreatedAtAction(
            nameof(GetTechnologyBySlug),
            new { slug = createdTechnology.Slug },
            createdTechnology);
    }

    /// <summary>
    /// Updates an existing technology.
    /// </summary>
    /// <param name="id">The unique identifier of the technology to update.</param>
    /// <param name="updateTechnologyDto">The updated technology data.</param>
    /// <returns>The updated technology if found.</returns>
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateTechnologyDto>))]
    [ProducesResponseType(typeof(TechnologyReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TechnologyReadDto>> UpdateTechnology(int id, [FromBody] UpdateTechnologyDto updateTechnologyDto)
    {
        var updatedTechnology = await _technologyService.UpdateTechnologyAsync(id, updateTechnologyDto);

        if (updatedTechnology is null)
        {
            return NotFound();
        }
        return Ok(updatedTechnology);
    }

    /// <summary>
    /// Deletes a technology by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the technology to delete.</param>
    /// <returns>No content if the technology was deleted; otherwise not found.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTechnology(int id)
    {
        var deleted = await _technologyService.DeleteTechnologyAsync(id);

        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
