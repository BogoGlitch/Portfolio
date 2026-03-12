using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos;
using Portfolio.Api.Interfaces;

namespace Portfolio.Api.Services;

public class ProjectService : IProjectService
{

    private readonly AppDbContext _context;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(AppDbContext context, ILogger<ProjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsAsync()
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Retrieving all projects.");
        }

        return await _context.Projects
            .AsNoTracking()
            .OrderBy(project => project.DisplayOrder)
            .ThenBy(project => project.Name)
            .Select(ProjectProjections.ToDto())
            .ToListAsync();
    }

    public async Task<ProjectDto?> GetProjectBySlugAsync(string slug)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Retrieving project with slug {ProjectSlug}.", slug);
        }

        return await _context.Projects
            .AsNoTracking()
            .Where(project => project.Slug == slug)
            .Select(ProjectProjections.ToDto())
            .FirstOrDefaultAsync();
    }
}
