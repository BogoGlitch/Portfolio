using Portfolio.Api.Dtos.Skills;

namespace Portfolio.Api.Features.Skills.Commands.CreateSkill;

/// <summary>
/// Represents a request to create a new skill.
/// </summary>
public record CreateSkillCommand(CreateSkillDto Dto);
