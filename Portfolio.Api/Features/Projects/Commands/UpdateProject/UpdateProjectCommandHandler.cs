using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Features.Projects.Commands.UpdateProject;

/// <summary>
/// Handles the UpdateProjectCommand. Returns null if the project does not exist,
/// allowing the controller to produce a 404 without throwing an exception.
/// </summary>
public class UpdateProjectCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(AppDbContext db, ILogger<UpdateProjectCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ProjectReadDto?> HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        // Load with ProjectTechnologies so EF tracks the collection and we can replace it.
        var project = await _db.Projects
            .Include(p => p.ProjectTechnologies)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (project is null)
        {
            return null;
        }

        var dto = command.Dto;
        var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();

        // Slug uniqueness check excludes the current project so updating without
        // changing the slug does not incorrectly trigger a conflict.
        var slugExists = await _db.Projects
            .AnyAsync(p => p.Slug == normalizedSlug && p.Id != command.Id, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{normalizedSlug}' already exists.");
        }

        var technologies = await _db.Technologies
            .Where(t => dto.TechnologyIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        if (technologies.Count != dto.TechnologyIds.Count)
        {
            throw new InvalidOperationException("One or more technology IDs are invalid.");
        }

        project.Name = dto.Name;
        project.Slug = normalizedSlug;
        project.ShortDescription = dto.ShortDescription;
        project.FullDescription = dto.FullDescription;
        project.RepoUrl = dto.RepoUrl;
        project.LiveUrl = dto.LiveUrl;
        project.ImageUrl = dto.ImageUrl;
        project.IsFeatured = dto.IsFeatured;
        project.DisplayOrder = dto.DisplayOrder;

        // Replace the technology collection by clearing and re-adding.
        project.ProjectTechnologies.Clear();
        foreach (var technology in technologies)
        {
            project.ProjectTechnologies.Add(new ProjectTechnology
            {
                ProjectId = project.Id,
                TechnologyId = technology.Id
            });
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated project {ProjectId}.", project.Id);

        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == project.Id)
            .Select(ProjectProjections.ToDto())
            .FirstAsync(cancellationToken);
    }
}
