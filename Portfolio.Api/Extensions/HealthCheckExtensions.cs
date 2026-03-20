using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Portfolio.Api.Extensions;

/// <summary>
/// Maps the health check endpoints onto the application pipeline.
///
/// Two endpoints are exposed:
///   GET /health      — full check: app + all registered dependencies (database, etc.)
///   GET /health/live — liveness only: confirms the process is running, no dependency checks
///
/// The /health/live endpoint is useful for infrastructure that just needs to know
/// the process is alive (e.g. Kubernetes liveness probes) without waiting on DB queries.
///
/// Responses are JSON so they can be parsed by monitoring tools and CI/CD pipelines.
/// </summary>
public static class HealthCheckExtensions
{
    public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder app)
    {
        // Full health check — runs all registered checks including the database
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteJsonResponse
        });

        // Liveness check — only checks that the process is up, skips dependency checks
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // skip all registered checks
            ResponseWriter = WriteJsonResponse
        });

        return app;
    }

    /// <summary>
    /// Writes the health check result as structured JSON.
    /// The default response is plain text ("Healthy") — this gives consumers
    /// a parseable shape with individual check results and timings.
    /// </summary>
    private static async Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration.ToString()
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        );
    }
}
