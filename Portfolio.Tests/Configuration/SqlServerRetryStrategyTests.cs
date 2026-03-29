using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Portfolio.Api.Data;

namespace Portfolio.Tests.Configuration;

public class SqlServerRetryStrategyTests
{
    [Fact]
    public void DbContext_UsesRetryingExecutionStrategy_WhenConfiguredWithSqlServer()
    {
        // Arrange — mirror the production configuration from Program.cs
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(
                "Server=fake;Database=fake;",
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null))
            .Options;

        using var context = new AppDbContext(options);

        // Act
        var strategy = context.Database.CreateExecutionStrategy();

        // Assert
        strategy.Should().BeOfType<SqlServerRetryingExecutionStrategy>();
    }

    [Fact]
    public void DbContext_WithoutRetry_DoesNotUseRetryingStrategy()
    {
        // Arrange — no retry configured (what we had before)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=fake;Database=fake;")
            .Options;

        using var context = new AppDbContext(options);

        // Act
        var strategy = context.Database.CreateExecutionStrategy();

        // Assert — default strategy is NOT the retrying one
        strategy.Should().NotBeOfType<SqlServerRetryingExecutionStrategy>();
    }
}
