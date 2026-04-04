using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Features.Projects.Commands.CreateProject;

/// <summary>
/// Handles the CreateProjectCommand. Validates slug uniqueness and skill IDs,
/// persists the new project, then re-fetches it with full navigation data for the response.
/// </summary>
public class CreateProjectCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(AppDbContext db, ILogger<CreateProjectCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ProjectReadDto> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        var dto = command.Dto;
        var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();

        // Slugs must be unique across all projects.
        var slugExists = await _db.Projects
            .AnyAsync(p => p.Slug == normalizedSlug, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{normalizedSlug}' already exists.");
        }

        // Validate that every supplied skill ID actually exists in the database.
        // If the counts differ, at least one ID was invalid.
        var skills = await _db.Skills
            .Where(s => dto.SkillIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (skills.Count != dto.SkillIds.Count)
        {
            throw new InvalidOperationException("One or more skill IDs are invalid.");
        }

        var project = new Project
        {
            Name = dto.Name,
            Slug = normalizedSlug,
            ShortDescription = dto.ShortDescription,
            FullDescription = dto.FullDescription,
            RepoUrl = dto.RepoUrl,
            LiveUrl = dto.LiveUrl,
            ImageUrl = dto.ImageUrl,
            IsFeatured = dto.IsFeatured,
            DisplayOrder = dto.DisplayOrder,
            DateCreatedUtc = DateTime.UtcNow,
            ProjectSkills = skills
                .Select(s => new ProjectSkill { SkillId = s.Id })
                .ToList()
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created project {ProjectId} with slug {ProjectSlug}.", project.Id, project.Slug);

        // Re-fetch with AsNoTracking so the response includes full navigation data
        // (e.g. skill names) that EF does not populate on the tracked entity after insert.
        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == project.Id)
            .Select(ProjectProjections.ToDto())
            .FirstAsync(cancellationToken);
    }
}
