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
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .Select(ProjectProjections.ToDto())
            .ToListAsync();
    }

    public async Task<ProjectReadDto?> GetProjectBySlugAsync(string slug)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Retrieving project with slug {ProjectSlug}.", slug);
        }

        slug = slug.Trim().ToLowerInvariant();

        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.Slug == slug)
            .Select(ProjectProjections.ToDto())
            .FirstOrDefaultAsync();
    }

    public async Task<ProjectReadDto> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        var normalizedSlug = createProjectDto.Slug.Trim().ToLowerInvariant();

        var slugExists = await _context.Projects
            .AnyAsync(p => p.Slug == normalizedSlug);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{normalizedSlug}' already exists.");
        }

        var project = new Project
        {
            Name = createProjectDto.Name,
            Slug = normalizedSlug,
            ShortDescription = createProjectDto.ShortDescription,
            FullDescription = createProjectDto.FullDescription,
            RepoUrl = createProjectDto.RepoUrl,
            LiveUrl = createProjectDto.LiveUrl,
            ImageUrl = createProjectDto.ImageUrl,
            IsFeatured = createProjectDto.IsFeatured,
            DisplayOrder = createProjectDto.DisplayOrder,
            DateCreatedUtc = DateTime.UtcNow
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return ProjectProjections.ToDto().Compile()(project);
    }

    public async Task<ProjectReadDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return null;
        }

        var normalizedSlug = updateProjectDto.Slug.Trim().ToLowerInvariant();

        var slugExists = await _context.Projects
            .AnyAsync(p => p.Slug == normalizedSlug && p.Id != id);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{normalizedSlug}' already exists.");
        }

        project.Name = updateProjectDto.Name;
        project.Slug = normalizedSlug;
        project.ShortDescription = updateProjectDto.ShortDescription;
        project.FullDescription = updateProjectDto.FullDescription;
        project.RepoUrl = updateProjectDto.RepoUrl;
        project.LiveUrl = updateProjectDto.LiveUrl;
        project.ImageUrl = updateProjectDto.ImageUrl;
        project.IsFeatured = updateProjectDto.IsFeatured;
        project.DisplayOrder = updateProjectDto.DisplayOrder;

        await _context.SaveChangesAsync();

        return ProjectProjections.ToDto().Compile()(project);

    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project is null)
        {
            return false;
        }

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return true;

    }
}
