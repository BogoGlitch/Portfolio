using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Features.Projects.Queries.GetProjectBySlug;

/// <summary>
/// Represents a request to retrieve a single project by its slug.
/// Returns null if no project with the given slug exists.
/// </summary>
public record GetProjectBySlugQuery(string Slug);
