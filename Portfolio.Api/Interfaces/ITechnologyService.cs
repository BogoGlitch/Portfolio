using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Interfaces;

public interface ITechnologyService
{
    Task<IEnumerable<TechnologyReadDto>> GetTechnologiesAsync();
    Task<TechnologyReadDto?> GetTechnologyBySlugAsync(string slug);
    Task<TechnologyReadDto> CreateTechnologyAsync(CreateTechnologyDto createTechnologyDto);
    Task<TechnologyReadDto?> UpdateTechnologyAsync(int id, UpdateTechnologyDto updateTechnologyDto);
    Task<bool> DeleteTechnologyAsync(int id);
}
