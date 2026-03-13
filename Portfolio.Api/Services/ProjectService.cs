using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Entities;
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

    public async Task<IEnumerable<ProjectReadDto>> GetProjectsAsync()
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

    public async Task<ProjectReadDto?> GetProjectBySlugAsync(string slug)
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

    public async Task<ProjectReadDto> CreateProjectAsync(CreateProjectDto createProjectDto)
    {

        var slugExists = await _context.Projects
            .AnyAsync(p => p.Slug == createProjectDto.Slug);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{createProjectDto.Slug}' already exists.");
        }

        var project = new Project
        {
            Name = createProjectDto.Name,
            Slug = createProjectDto.Slug,
            ShortDescription = createProjectDto.ShortDescription,
            FullDescription = createProjectDto.FullDescription,
            RepoUrl = createProjectDto.RepoUrl,
            LiveUrl = createProjectDto.LiveUrl,
            ImageUrl = createProjectDto.ImageUrl,
            IsFeatured = false,
            DateCreatedUtc = DateTime.UtcNow
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return ProjectProjections.ToDto().Compile()(project);
    }

    public async Task<ProjectReadDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return null;
        }

        project.Name = updateProjectDto.Name;
        project.Slug = updateProjectDto.Slug;
        project.ShortDescription = updateProjectDto.ShortDescription;
        project.FullDescription = updateProjectDto.FullDescription;
        project.RepoUrl = updateProjectDto.RepoUrl;
        project.LiveUrl = updateProjectDto.LiveUrl;
        project.ImageUrl = updateProjectDto.ImageUrl;

        await _context.SaveChangesAsync();

        return ProjectProjections.ToDto().Compile()(project);

    }
}
