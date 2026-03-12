using Portfolio.Api.Dtos;

namespace Portfolio.Api.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsAsync();
    Task<ProjectDto?> GetProjectBySlugAsync(string slug);
}
