using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Projects.Commands.DeleteProject;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Projects;

public class DeleteProjectCommandHandlerTests
{
    private static DeleteProjectCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new DeleteProjectCommandHandler(db, NullLogger<DeleteProjectCommandHandler>.Instance);
    }

    [Fact]
    public async Task NotFound_ReturnsFalse()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsFalse));

        var result = await handler.HandleAsync(new DeleteProjectCommand(999));

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Found_ReturnsTrue()
    {
        var db = DbContextFactory.Create(nameof(Found_ReturnsTrue));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new DeleteProjectCommandHandler(db, NullLogger<DeleteProjectCommandHandler>.Instance);

        var result = await handler.HandleAsync(new DeleteProjectCommand(1));

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Found_RemovesFromDatabase()
    {
        var db = DbContextFactory.Create(nameof(Found_RemovesFromDatabase));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new DeleteProjectCommandHandler(db, NullLogger<DeleteProjectCommandHandler>.Instance);

        await handler.HandleAsync(new DeleteProjectCommand(1));

        db.Projects.Should().BeEmpty();
    }
}
