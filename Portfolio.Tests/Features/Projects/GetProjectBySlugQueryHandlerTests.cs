using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Projects.Queries.GetProjectBySlug;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Projects;

public class GetProjectBySlugQueryHandlerTests
{
    private static GetProjectBySlugQueryHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new GetProjectBySlugQueryHandler(db, NullLogger<GetProjectBySlugQueryHandler>.Instance);
    }

    [Fact]
    public async Task NotFound_ReturnsNull()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsNull));

        var result = await handler.HandleAsync(new GetProjectBySlugQuery("does-not-exist"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Found_ReturnsDto()
    {
        var db = DbContextFactory.Create(nameof(Found_ReturnsDto));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new GetProjectBySlugQueryHandler(db, NullLogger<GetProjectBySlugQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetProjectBySlugQuery("my-project"));

        result.Should().NotBeNull();
        result!.Name.Should().Be("My Project");
        result.Slug.Should().Be("my-project");
    }

    [Fact]
    public async Task Slug_IsNormalized_BeforeQuery()
    {
        // Handler normalizes the incoming slug so a mixed-case lookup still resolves.
        var db = DbContextFactory.Create(nameof(Slug_IsNormalized_BeforeQuery));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new GetProjectBySlugQueryHandler(db, NullLogger<GetProjectBySlugQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetProjectBySlugQuery("  My-Project  "));

        result.Should().NotBeNull();
        result!.Slug.Should().Be("my-project");
    }
}
