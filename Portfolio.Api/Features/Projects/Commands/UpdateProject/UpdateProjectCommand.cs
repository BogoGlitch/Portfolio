using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Represents a request to update an existing project by its ID.
/// </summary>
public record UpdateProjectCommand(int Id, UpdateProjectDto Dto);
