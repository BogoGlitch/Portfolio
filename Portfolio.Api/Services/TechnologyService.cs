using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos;
using Portfolio.Api.Interfaces;

namespace Portfolio.Api.Services;

public class TechnologyService : ITechnologyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TechnologyService> _logger;

    public TechnologyService(AppDbContext context, ILogger<TechnologyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TechnologyDto>> GetTechnologiesAsync()
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Fetching all technologies.");
        }

        return await _context.Technologies
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(TechnologyProjections.ToDto)
            .ToListAsync();
    }

    public async Task<TechnologyDto?> GetTechnologyBySlugAsync(string slug)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Fetching technology by slug: {Slug}", slug);
        }

        slug = slug.ToLowerInvariant();

        return await _context.Technologies
            .AsNoTracking()
            .Where(t => t.Slug == slug)
            .Select(TechnologyProjections.ToDto)
            .FirstOrDefaultAsync();
    }
}
