using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Technologies.Queries.GetTechnologyBySlug;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Technologies;

public class GetTechnologyBySlugQueryHandlerTests
{
    private static GetTechnologyBySlugQueryHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new GetTechnologyBySlugQueryHandler(db, NullLogger<GetTechnologyBySlugQueryHandler>.Instance);
    }

    [Fact]
    public async Task NotFound_ReturnsNull()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsNull));

        var result = await handler.HandleAsync(new GetTechnologyBySlugQuery("does-not-exist"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Found_ReturnsDto()
    {
        var db = DbContextFactory.Create(nameof(Found_ReturnsDto));
        db.Technologies.Add(new Technology { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new GetTechnologyBySlugQueryHandler(db, NullLogger<GetTechnologyBySlugQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetTechnologyBySlugQuery("dotnet"));

        result.Should().NotBeNull();
        result!.Name.Should().Be(".NET");
        result.Slug.Should().Be("dotnet");
    }

    [Fact]
    public async Task Slug_IsNormalized_BeforeQuery()
    {
        // Handler normalizes the incoming slug so a mixed-case lookup still resolves.
        var db = DbContextFactory.Create(nameof(Slug_IsNormalized_BeforeQuery));
        db.Technologies.Add(new Technology { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new GetTechnologyBySlugQueryHandler(db, NullLogger<GetTechnologyBySlugQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetTechnologyBySlugQuery("  DotNet  "));

        result.Should().NotBeNull();
        result!.Slug.Should().Be("dotnet");
    }
}
