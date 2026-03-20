namespace Portfolio.Api.Features.Technologies.Queries.GetTechnologyBySlug;

/// <summary>
/// Represents a request to retrieve a single technology by its slug.
/// Returns null if no technology with the given slug exists.
/// </summary>
public record GetTechnologyBySlugQuery(string Slug);
