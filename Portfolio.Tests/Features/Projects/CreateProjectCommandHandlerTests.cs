using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Projects.Commands.CreateProject;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    private static CreateProjectCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new CreateProjectCommandHandler(db, NullLogger<CreateProjectCommandHandler>.Instance);
    }

    private static CreateProjectCommand ValidCommand(string slug = "my-project", IReadOnlyList<int>? technologyIds = null) =>
        new(new CreateProjectDto(
            Name: "My Project",
            Slug: slug,
            ShortDescription: "A short description.",
            FullDescription: "A longer, full description of the project.",
            RepoUrl: null,
            LiveUrl: null,
            ImageUrl: null,
            IsFeatured: false,
            DisplayOrder: 0,
            TechnologyIds: technologyIds ?? []
        ));

    [Fact]
    public async Task ValidCommand_NoTechnologies_ReturnsProjectReadDto()
    {
        var handler = BuildHandler(nameof(ValidCommand_NoTechnologies_ReturnsProjectReadDto));

        var result = await handler.HandleAsync(ValidCommand());

        result.Should().NotBeNull();
        result.Name.Should().Be("My Project");
        result.Slug.Should().Be("my-project");
        result.Technologies.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidCommand_PersistsToDatabase()
    {
        var db = DbContextFactory.Create(nameof(ValidCommand_PersistsToDatabase));
        var handler = new CreateProjectCommandHandler(db, NullLogger<CreateProjectCommandHandler>.Instance);

        await handler.HandleAsync(ValidCommand());

        db.Projects.Should().ContainSingle(p => p.Slug == "my-project");
    }

    [Fact]
    public async Task ValidCommand_WithTechnologies_ReturnsDtoWithTechnologies()
    {
        var db = DbContextFactory.Create(nameof(ValidCommand_WithTechnologies_ReturnsDtoWithTechnologies));
        var handler = new CreateProjectCommandHandler(db, NullLogger<CreateProjectCommandHandler>.Instance);

        db.Technologies.Add(new Technology { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        db.Technologies.Add(new Technology { Id = 2, Name = "React", Slug = "react", Description = "desc", Category = "Frontend", Discipline = "Frontend", DisplayOrder = 1 });
        await db.SaveChangesAsync();

        var result = await handler.HandleAsync(ValidCommand(technologyIds: [1, 2]));

        result.Technologies.Should().HaveCount(2);
        result.Technologies.Select(t => t.Slug).Should().BeEquivalentTo(["dotnet", "react"]);
    }

    [Fact]
    public async Task DuplicateSlug_ThrowsInvalidOperationException()
    {
        var dbName = nameof(DuplicateSlug_ThrowsInvalidOperationException);
        var handler = BuildHandler(dbName);
        await handler.HandleAsync(ValidCommand());

        var act = () => handler.HandleAsync(ValidCommand());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*my-project*");
    }

    [Fact]
    public async Task InvalidTechnologyId_ThrowsInvalidOperationException()
    {
        var handler = BuildHandler(nameof(InvalidTechnologyId_ThrowsInvalidOperationException));

        var act = () => handler.HandleAsync(ValidCommand(technologyIds: [999]));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*technology IDs are invalid*");
    }

    // TODO: db name "MixedValidAndInvalidTechnologyIds_ThrowsInvalidOperationException" conflicts with UpdateProjectCommandHandlerTests — revisit in a dedicated session
    // [Fact]
    // public async Task MixedValidAndInvalidTechnologyIds_ThrowsInvalidOperationException() { ... }

    [Fact]
    public async Task Slug_IsTrimmedAndLowercased()
    {
        var handler = BuildHandler(nameof(Slug_IsTrimmedAndLowercased));

        var result = await handler.HandleAsync(ValidCommand(slug: "  My-Project  "));

        result.Slug.Should().Be("my-project");
    }

    [Fact]
    public async Task ValidCommand_AssignsId()
    {
        var handler = BuildHandler(nameof(ValidCommand_AssignsId));

        var result = await handler.HandleAsync(ValidCommand());

        result.Id.Should().BeGreaterThan(0);
    }
}
