using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Skills;

namespace Portfolio.Api.Features.Skills.Queries.GetSkills;

/// <summary>
/// Handles the GetSkillsQuery. Returns all skills ordered by DisplayOrder then Name.
/// </summary>
public class GetSkillsQueryHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetSkillsQueryHandler> _logger;

    public GetSkillsQueryHandler(AppDbContext db, ILogger<GetSkillsQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<SkillReadDto>> HandleAsync(GetSkillsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all skills.");

        return await _db.Skills
            .AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .Select(SkillProjections.ToDto())
            .ToListAsync(cancellationToken);
    }
}
