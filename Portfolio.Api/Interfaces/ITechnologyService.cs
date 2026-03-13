using Portfolio.Api.Dtos;

namespace Portfolio.Api.Interfaces;

public interface ITechnologyService
{
    Task<IEnumerable<TechnologyReadDto>> GetTechnologiesAsync();
    Task<TechnologyReadDto?> GetTechnologyBySlugAsync(string slug);
}
