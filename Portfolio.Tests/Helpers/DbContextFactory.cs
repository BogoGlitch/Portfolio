using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;

namespace Portfolio.Tests.Helpers;

/// <summary>
/// Creates a fresh InMemory AppDbContext for each test.
/// Each call gets its own database name so tests never share state.
/// </summary>
public static class DbContextFactory
{
    public static AppDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
