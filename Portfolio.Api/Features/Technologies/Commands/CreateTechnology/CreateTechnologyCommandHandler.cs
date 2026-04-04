using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Features.Technologies.Commands.CreateTechnology;

/// <summary>
/// Handles the CreateTechnologyCommand. Validates slug uniqueness,
/// persists the new technology, then re-fetches it for the response.
/// </summary>
public class CreateTechnologyCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateTechnologyCommandHandler> _logger;

    public CreateTechnologyCommandHandler(AppDbContext db, ILogger<CreateTechnologyCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<TechnologyReadDto> HandleAsync(CreateTechnologyCommand command, CancellationToken cancellationToken = default)
    {
        var dto = command.Dto;
        var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();

        var slugExists = await _db.Technologies
            .AnyAsync(t => t.Slug == normalizedSlug, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A technology with slug '{normalizedSlug}' already exists.");
        }

        var technology = new Technology
        {
            Name = dto.Name,
            Slug = normalizedSlug,
            Description = dto.Description,
            Category = dto.Category,
            Discipline = dto.Discipline,
            LogoUrl = dto.LogoUrl,
            DocumentationUrl = dto.DocumentationUrl,
            FullStory = dto.FullStory,
            IsFeatured = dto.IsFeatured,
            DisplayOrder = dto.DisplayOrder,
            DateAddedUtc = DateTime.UtcNow
        };

        _db.Technologies.Add(technology);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created technology {TechnologyId} with slug {TechnologySlug}.", technology.Id, technology.Slug);

        // Re-fetch with AsNoTracking so the projection runs cleanly against the database.
        return await _db.Technologies
            .AsNoTracking()
            .Where(t => t.Id == technology.Id)
            .Select(TechnologyProjections.ToDto())
            .FirstAsync(cancellationToken);
    }
}
