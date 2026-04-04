using Portfolio.Api.Dtos.Skills;

namespace Portfolio.Api.Features.Skills.Commands.UpdateSkill;

/// <summary>
/// Represents a request to update an existing skill by its ID.
/// </summary>
public record UpdateSkillCommand(int Id, UpdateSkillDto Dto);
