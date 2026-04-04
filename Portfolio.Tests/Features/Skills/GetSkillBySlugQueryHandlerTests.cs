using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Skills.Queries.GetSkillBySlug;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Skills;

public class GetSkillBySlugQueryHandlerTests
{
    private static GetSkillBySlugQueryHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new GetSkillBySlugQueryHandler(db, NullLogger<GetSkillBySlugQueryHandler>.Instance);
    }

    [Fact]
    public async Task NotFound_ReturnsNull()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsNull));

        var result = await handler.HandleAsync(new GetSkillBySlugQuery("does-not-exist"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Found_ReturnsDto()
    {
        var db = DbContextFactory.Create(nameof(Found_ReturnsDto));
        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new GetSkillBySlugQueryHandler(db, NullLogger<GetSkillBySlugQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetSkillBySlugQuery("dotnet"));

        result.Should().NotBeNull();
        result!.Name.Should().Be(".NET");
        result.Slug.Should().Be("dotnet");
    }

    [Fact]
    public async Task Slug_IsNormalized_BeforeQuery()
    {
        // Handler normalizes the incoming slug so a mixed-case lookup still resolves.
        var db = DbContextFactory.Create(nameof(Slug_IsNormalized_BeforeQuery));
        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new GetSkillBySlugQueryHandler(db, NullLogger<GetSkillBySlugQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetSkillBySlugQuery("  DotNet  "));

        result.Should().NotBeNull();
        result!.Slug.Should().Be("dotnet");
    }
}
