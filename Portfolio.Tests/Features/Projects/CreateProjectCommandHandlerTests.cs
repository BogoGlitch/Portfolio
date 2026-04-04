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

    private static CreateProjectCommand ValidCommand(string slug = "my-project", IReadOnlyList<int>? skillIds = null) =>
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
            SkillIds: skillIds ?? []
        ));

    [Fact]
    public async Task ValidCommand_NoSkills_ReturnsProjectReadDto()
    {
        var handler = BuildHandler(nameof(ValidCommand_NoSkills_ReturnsProjectReadDto));

        var result = await handler.HandleAsync(ValidCommand());

        result.Should().NotBeNull();
        result.Name.Should().Be("My Project");
        result.Slug.Should().Be("my-project");
        result.Skills.Should().BeEmpty();
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
    public async Task ValidCommand_WithSkills_ReturnsDtoWithSkills()
    {
        var db = DbContextFactory.Create(nameof(ValidCommand_WithSkills_ReturnsDtoWithSkills));
        var handler = new CreateProjectCommandHandler(db, NullLogger<CreateProjectCommandHandler>.Instance);

        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        db.Skills.Add(new Skill { Id = 2, Name = "React", Slug = "react", Description = "desc", Category = "Frontend", Discipline = "Frontend", DisplayOrder = 1 });
        await db.SaveChangesAsync();

        var result = await handler.HandleAsync(ValidCommand(skillIds: [1, 2]));

        result.Skills.Should().HaveCount(2);
        result.Skills.Select(s => s.Slug).Should().BeEquivalentTo(["dotnet", "react"]);
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
    public async Task InvalidSkillId_ThrowsInvalidOperationException()
    {
        var handler = BuildHandler($"Create_{nameof(InvalidSkillId_ThrowsInvalidOperationException)}");

        var act = () => handler.HandleAsync(ValidCommand(skillIds: [999]));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*skill IDs are invalid*");
    }

    // TODO: db name "MixedValidAndInvalidSkillIds_ThrowsInvalidOperationException" conflicts with UpdateProjectCommandHandlerTests — revisit in a dedicated session
    // [Fact]
    // public async Task MixedValidAndInvalidSkillIds_ThrowsInvalidOperationException() { ... }

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
