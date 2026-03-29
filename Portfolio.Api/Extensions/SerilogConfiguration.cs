using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Portfolio.Api.Extensions;

/// <summary>
/// Configures Serilog as the application logger.
///
/// Two sinks are registered:
///   - Console: human-readable output for local development
///   - File: rolling daily log files under /logs for persistence
///
/// Log level overrides keep EF Core and ASP.NET Core infrastructure noise
/// at Warning in production so your own handler logs remain visible.
/// </summary>
public static class SerilogConfiguration
{
    public static void Configure(HostBuilderContext context, IServiceProvider services, LoggerConfiguration config)
    {
        config
            .MinimumLevel.Information()

            // Suppress noisy framework logs — we only want to see our own code at Information.
            // EF Core logs every SQL query at Information by default; this keeps them quiet
            // unless something goes wrong.
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)

            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Portfolio.Api")
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)

            // Console sink — readable output during local development
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            )

            // File sink — one log file per day, kept for 14 days
            .WriteTo.File(
                path: "logs/portfolio-api-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            );

        // Application Insights sink — sends Serilog events to Azure in non-Development environments.
        // The SDK auto-collects requests, dependencies, and exceptions; this sink adds structured log events.
        if (!context.HostingEnvironment.IsDevelopment())
        {
            var telemetryClient = services.GetRequiredService<TelemetryClient>();
            config.WriteTo.ApplicationInsights(telemetryClient, TelemetryConverter.Traces);
        }
    }
}
