using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Skills;

namespace Portfolio.Api.Features.Skills.Queries.GetSkillBySlug;

/// <summary>
/// Handles the GetSkillBySlugQuery. Normalises the slug to lowercase
/// before querying so lookups are case-insensitive by convention.
/// </summary>
public class GetSkillBySlugQueryHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetSkillBySlugQueryHandler> _logger;

    public GetSkillBySlugQueryHandler(AppDbContext db, ILogger<GetSkillBySlugQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<SkillReadDto?> HandleAsync(GetSkillBySlugQuery query, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = query.Slug.Trim().ToLowerInvariant();

        _logger.LogInformation("Retrieving skill with slug {SkillSlug}.", normalizedSlug);

        return await _db.Skills
            .AsNoTracking()
            .Where(s => s.Slug == normalizedSlug)
            .Select(SkillProjections.ToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }
}
