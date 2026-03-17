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

        var technologies = await _context.Technologies
            .Where(t => createProjectDto.TechnologyIds.Contains(t.Id))
            .ToListAsync();

        if (technologies.Count != createProjectDto.TechnologyIds.Count)
        {
            throw new InvalidOperationException("One or more technology IDs are invalid.");
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
            DateCreatedUtc = DateTime.UtcNow,

            ProjectTechnologies = technologies
                .Select(technologies => new ProjectTechnology
                {
                    TechnologyId = technologies.Id,
                })
                .ToList()
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == project.Id)
            .Select(ProjectProjections.ToDto())
            .FirstAsync();
    }

    public async Task<ProjectReadDto?> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectTechnologies)
            .FirstOrDefaultAsync(p => p.Id == id);

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

        var technologies = await _context.Technologies
            .Where(t => updateProjectDto.TechnologyIds.Contains(t.Id))
            .ToListAsync();

        if (technologies.Count != updateProjectDto.TechnologyIds.Count)
        {
            throw new InvalidOperationException("One or more technology IDs are invalid.");
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

        project.ProjectTechnologies.Clear();
        foreach (var technology in technologies)
        {
            project.ProjectTechnologies.Add(new ProjectTechnology
            {
                ProjectId = project.Id,
                TechnologyId = technology.Id
            });
        }

        await _context.SaveChangesAsync();

        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == project.Id)
            .Select(ProjectProjections.ToDto())
            .FirstAsync();
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
