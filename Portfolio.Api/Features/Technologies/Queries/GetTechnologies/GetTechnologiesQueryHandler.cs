using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Features.Technologies.Queries.GetTechnologies;

/// <summary>
/// Handles the GetTechnologiesQuery. Returns all technologies ordered by DisplayOrder then Name.
/// </summary>
public class GetTechnologiesQueryHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetTechnologiesQueryHandler> _logger;

    public GetTechnologiesQueryHandler(AppDbContext db, ILogger<GetTechnologiesQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<TechnologyReadDto>> HandleAsync(GetTechnologiesQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all technologies.");

        return await _db.Technologies
            .AsNoTracking()
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.Name)
            .Select(TechnologyProjections.ToDto())
            .ToListAsync(cancellationToken);
    }
}
