using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;

namespace Portfolio.Api.Services;

/// <summary>
/// Runs a cheap SELECT 1 query every 4 minutes to prevent Azure SQL Basic
/// from going idle and causing 29-second login timeouts on the next real request.
/// </summary>
public class DatabaseKeepAliveService(IServiceScopeFactory scopeFactory, ILogger<DatabaseKeepAliveService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(4);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(Interval, stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.ExecuteSqlRawAsync("SELECT 1", stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Database keep-alive ping failed.");
            }
        }
    }
}
