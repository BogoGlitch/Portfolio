using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Features.Technologies.Queries.GetTechnologyBySlug;

/// <summary>
/// Handles the GetTechnologyBySlugQuery. Normalises the slug to lowercase
/// before querying so lookups are case-insensitive by convention.
/// </summary>
public class GetTechnologyBySlugQueryHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetTechnologyBySlugQueryHandler> _logger;

    public GetTechnologyBySlugQueryHandler(AppDbContext db, ILogger<GetTechnologyBySlugQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<TechnologyReadDto?> HandleAsync(GetTechnologyBySlugQuery query, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = query.Slug.Trim().ToLowerInvariant();

        _logger.LogInformation("Retrieving technology with slug {TechnologySlug}.", normalizedSlug);

        return await _db.Technologies
            .AsNoTracking()
            .Where(t => t.Slug == normalizedSlug)
            .Select(TechnologyProjections.ToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }
}
