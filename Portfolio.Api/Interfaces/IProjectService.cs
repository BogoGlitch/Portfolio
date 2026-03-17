using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectReadDto>> GetProjectsAsync(ProjectQueryParametersDto queryParameters);
    Task<ProjectReadDto?> GetProjectBySlugAsync(string slug);
    Task<ProjectReadDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ProjectReadDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto);
    Task<bool> DeleteProjectAsync(int id);
}
