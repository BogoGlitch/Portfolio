using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;

namespace Portfolio.Api.Services;

/// <summary>
/// Runs the main read queries once at startup so EF Core compiles
/// the expression trees before the first real user request arrives.
/// </summary>
public class QueryWarmupService(IServiceScopeFactory scopeFactory, ILogger<QueryWarmupService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Warm up the Projects query (includes ProjectTechnologies → Technology)
            await db.Projects
                .AsNoTracking()
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .Select(ProjectProjections.ToDto())
                .ToListAsync(stoppingToken);

            // Warm up the Technologies query (includes ProjectTechnologies → Project)
            await db.Technologies
                .AsNoTracking()
                .OrderBy(t => t.DisplayOrder)
                .ThenBy(t => t.Name)
                .Select(TechnologyProjections.ToDto())
                .ToListAsync(stoppingToken);

            logger.LogInformation("Query warmup completed — Projects and Technologies queries compiled.");
        }
        catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "Query warmup failed — first user request will compile queries.");
        }
    }
}
