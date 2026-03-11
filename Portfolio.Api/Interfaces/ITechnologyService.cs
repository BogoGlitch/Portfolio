using Portfolio.Api.Dtos;

namespace Portfolio.Api.Interfaces;

public interface ITechnologyService
{
    Task<IEnumerable<TechnologyDto>> GetTechnologiesAsync();
    Task<TechnologyDto?> GetTechnologyBySlugAsync(string slug);
}
