using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TechnologyReadDto>> GetTechnologyBySlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest();
        }

        var technology = await _technologyService.GetTechnologyBySlugAsync(slug);
        if (technology == null)
        {
            return NotFound();
        }
        return Ok(technology);
    }
}
