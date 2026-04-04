using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Features.Projects.Queries.GetProjects;

/// <summary>
/// Handles the GetProjectsQuery. Applies optional skill ID filtering,
/// then returns all matching projects ordered by DisplayOrder then Name.
/// </summary>
public class GetProjectsQueryHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetProjectsQueryHandler> _logger;

    public GetProjectsQueryHandler(AppDbContext db, ILogger<GetProjectsQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectReadDto>> HandleAsync(GetProjectsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all projects.");

        // Parse the optional comma-separated skill ID filter from the query string.
        // e.g. "1,2,3" becomes [1, 2, 3]. Invalid values are silently skipped.
        var skillIds = new List<int>();

        if (!string.IsNullOrWhiteSpace(query.QueryParameters.SkillIds))
        {
            foreach (var value in query.QueryParameters.SkillIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(value, out var id))
                {
                    skillIds.Add(id);
                }
            }

            skillIds = skillIds.Distinct().ToList();
        }

        var dbQuery = _db.Projects.AsNoTracking();

        if (skillIds.Count > 0)
        {
            dbQuery = dbQuery.Where(p =>
                p.ProjectSkills.Any(ps => skillIds.Contains(ps.SkillId)));
        }

        if (!string.IsNullOrWhiteSpace(query.QueryParameters.Discipline))
        {
            var discipline = query.QueryParameters.Discipline.Trim();
            dbQuery = dbQuery.Where(p =>
                p.ProjectSkills.Any(ps => ps.Skill.Discipline == discipline));
        }

        return await dbQuery
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Name)
            .Select(ProjectProjections.ToDto())
            .ToListAsync(cancellationToken);
    }
}
