using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Features.Projects.Queries.GetProjects;

/// <summary>
/// Represents a request to retrieve all projects, with optional filtering by technology IDs.
/// </summary>
public record GetProjectsQuery(ProjectQueryParametersDto QueryParameters);
