using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Technologies.Commands.DeleteTechnology;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Technologies;

public class DeleteTechnologyCommandHandlerTests
{
    private static DeleteTechnologyCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new DeleteTechnologyCommandHandler(db, NullLogger<DeleteTechnologyCommandHandler>.Instance);
    }

    [Fact]
    public async Task NotFound_ReturnsFalse()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsFalse));

        var result = await handler.HandleAsync(new DeleteTechnologyCommand(999));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Found_ReturnsTrue()
    {
        var db = DbContextFactory.Create(nameof(Found_ReturnsTrue));
        db.Technologies.Add(new Technology { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new DeleteTechnologyCommandHandler(db, NullLogger<DeleteTechnologyCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DeleteTechnologyCommand(1));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Found_RemovesFromDatabase()
    {
        var db = DbContextFactory.Create(nameof(Found_RemovesFromDatabase));
        db.Technologies.Add(new Technology { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new DeleteTechnologyCommandHandler(db, NullLogger<DeleteTechnologyCommandHandler>.Instance);

        await handler.HandleAsync(new DeleteTechnologyCommand(1));

        db.Technologies.Should().BeEmpty();
    }
}
