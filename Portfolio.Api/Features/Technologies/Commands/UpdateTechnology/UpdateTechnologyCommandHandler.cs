using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Features.Technologies.Commands.UpdateTechnology;

/// <summary>
/// Handles the UpdateTechnologyCommand. Returns null if the technology does not exist,
/// allowing the controller to produce a 404 without throwing an exception.
/// </summary>
public class UpdateTechnologyCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<UpdateTechnologyCommandHandler> _logger;

    public UpdateTechnologyCommandHandler(AppDbContext db, ILogger<UpdateTechnologyCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<TechnologyReadDto?> HandleAsync(UpdateTechnologyCommand command, CancellationToken cancellationToken = default)
    {
        var technology = await _db.Technologies.FindAsync([command.Id], cancellationToken);

        if (technology is null)
        {
            return null;
        }

        var dto = command.Dto;
        var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();

        // Exclude the current technology from the uniqueness check so that updating
        // without changing the slug does not incorrectly trigger a conflict.
        var slugExists = await _db.Technologies
            .AnyAsync(t => t.Slug == normalizedSlug && t.Id != command.Id, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A technology with slug '{normalizedSlug}' already exists.");
        }

        technology.Name = dto.Name;
        technology.Slug = normalizedSlug;
        technology.Description = dto.Description;
        technology.Category = dto.Category;
        technology.Discipline = dto.Discipline;
        technology.LogoUrl = dto.LogoUrl;
        technology.DocumentationUrl = dto.DocumentationUrl;
        technology.FullStory = dto.FullStory;
        technology.IsFeatured = dto.IsFeatured;
        technology.DisplayOrder = dto.DisplayOrder;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated technology {TechnologyId}.", technology.Id);

        return await _db.Technologies
            .AsNoTracking()
            .Where(t => t.Id == technology.Id)
            .Select(TechnologyProjections.ToDto())
            .FirstAsync(cancellationToken);
    }
}
