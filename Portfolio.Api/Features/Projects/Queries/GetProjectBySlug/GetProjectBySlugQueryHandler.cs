using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Features.Projects.Queries.GetProjectBySlug;

/// <summary>
/// Handles the GetProjectBySlugQuery. Normalises the slug to lowercase
/// before querying so lookups are case-insensitive by convention.
/// </summary>
public class GetProjectBySlugQueryHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetProjectBySlugQueryHandler> _logger;

    public GetProjectBySlugQueryHandler(AppDbContext db, ILogger<GetProjectBySlugQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ProjectReadDto?> HandleAsync(GetProjectBySlugQuery query, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = query.Slug.Trim().ToLowerInvariant();

        _logger.LogInformation("Retrieving project with slug {ProjectSlug}.", normalizedSlug);

        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.Slug == normalizedSlug)
            .Select(ProjectProjections.ToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }
}
