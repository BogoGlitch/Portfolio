using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos.Skills;
using Portfolio.Api.Features.Skills.Commands.CreateSkill;
using Portfolio.Api.Features.Skills.Commands.DeleteSkill;
using Portfolio.Api.Features.Skills.Commands.UpdateSkill;
using Portfolio.Api.Features.Skills.Queries.GetSkills;
using Portfolio.Api.Features.Skills.Queries.GetSkillBySlug;
using Portfolio.Api.Filters;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SkillsController : ControllerBase
{
    private readonly GetSkillsQueryHandler _getSkills;
    private readonly GetSkillBySlugQueryHandler _getSkillBySlug;
    private readonly CreateSkillCommandHandler _createSkill;
    private readonly UpdateSkillCommandHandler _updateSkill;
    private readonly DeleteSkillCommandHandler _deleteSkill;

    public SkillsController(
        GetSkillsQueryHandler getSkills,
        GetSkillBySlugQueryHandler getSkillBySlug,
        CreateSkillCommandHandler createSkill,
        UpdateSkillCommandHandler updateSkill,
        DeleteSkillCommandHandler deleteSkill)
    {
        _getSkills = getSkills;
        _getSkillBySlug = getSkillBySlug;
        _createSkill = createSkill;
        _updateSkill = updateSkill;
        _deleteSkill = deleteSkill;
    }

    /// <summary>
    /// Retrieves all skills.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SkillReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SkillReadDto>>> GetSkills()
    {
        var result = await _getSkills.HandleAsync(new GetSkillsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a skill by its slug.
    /// </summary>
    [HttpGet("{slug:minlength(1)}")]
    [ProducesResponseType(typeof(SkillReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SkillReadDto>> GetSkillBySlug(string slug)
    {
        var result = await _getSkillBySlug.HandleAsync(new GetSkillBySlugQuery(slug));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Creates a new skill.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<CreateSkillDto>))]
    [ProducesResponseType(typeof(SkillReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SkillReadDto>> CreateSkill([FromBody] CreateSkillDto createSkillDto)
    {
        var result = await _createSkill.HandleAsync(new CreateSkillCommand(createSkillDto));
        return CreatedAtAction(nameof(GetSkillBySlug), new { slug = result.Slug }, result);
    }

    /// <summary>
    /// Updates an existing skill.
    /// </summary>
    [Authorize]
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter<UpdateSkillDto>))]
    [ProducesResponseType(typeof(SkillReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SkillReadDto>> UpdateSkill(int id, [FromBody] UpdateSkillDto updateSkillDto)
    {
        var result = await _updateSkill.HandleAsync(new UpdateSkillCommand(id, updateSkillDto));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes a skill by its unique identifier.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var deleted = await _deleteSkill.HandleAsync(new DeleteSkillCommand(id));
        return deleted ? NoContent() : NotFound();
    }
}
