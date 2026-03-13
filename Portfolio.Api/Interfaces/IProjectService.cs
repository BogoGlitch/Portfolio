using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectReadDto>> GetProjectsAsync();
    Task<ProjectReadDto?> GetProjectBySlugAsync(string slug);
    Task<ProjectReadDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ProjectReadDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto);
}
