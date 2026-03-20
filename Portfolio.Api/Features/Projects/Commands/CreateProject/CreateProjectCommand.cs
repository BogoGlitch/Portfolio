using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Features.Projects.Commands.CreateProject;

/// <summary>
/// Represents a request to create a new project.
/// </summary>
public record CreateProjectCommand(CreateProjectDto Dto);
