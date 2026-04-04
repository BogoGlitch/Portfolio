namespace Portfolio.Api.Features.Skills.Queries.GetSkillBySlug;

/// <summary>
/// Represents a request to retrieve a single skill by its slug.
/// Returns null if no skill with the given slug exists.
/// </summary>
public record GetSkillBySlugQuery(string Slug);
