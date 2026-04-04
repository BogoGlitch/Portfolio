using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Skills.Commands.DeleteSkill;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Skills;

public class DeleteSkillCommandHandlerTests
{
    private static DeleteSkillCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new DeleteSkillCommandHandler(db, NullLogger<DeleteSkillCommandHandler>.Instance);
    }

    [Fact]
    public async Task NotFound_ReturnsFalse()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsFalse));

        var result = await handler.HandleAsync(new DeleteSkillCommand(999));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Found_ReturnsTrue()
    {
        var db = DbContextFactory.Create(nameof(Found_ReturnsTrue));
        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new DeleteSkillCommandHandler(db, NullLogger<DeleteSkillCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DeleteSkillCommand(1));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Found_RemovesFromDatabase()
    {
        var db = DbContextFactory.Create(nameof(Found_RemovesFromDatabase));
        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new DeleteSkillCommandHandler(db, NullLogger<DeleteSkillCommandHandler>.Instance);

        await handler.HandleAsync(new DeleteSkillCommand(1));

        db.Skills.Should().BeEmpty();
    }
}
