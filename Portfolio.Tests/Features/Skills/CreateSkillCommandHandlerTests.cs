using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Dtos.Skills;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Skills.Commands.CreateSkill;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Skills;

public class CreateSkillCommandHandlerTests
{
    private static CreateSkillCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new CreateSkillCommandHandler(db, NullLogger<CreateSkillCommandHandler>.Instance);
    }

    private static CreateSkillCommand ValidCommand(string slug = "dotnet") =>
        new(new CreateSkillDto(
            Name: ".NET",
            Slug: slug,
            Description: "A cross-platform framework.",
            Category: "Backend",
            Discipline: "Backend",
            LogoUrl: null,
            DocumentationUrl: null,
            FullStory: null,
            IsFeatured: false,
            DisplayOrder: 0
        ));

    [Fact]
    public async Task ValidCommand_ReturnsSkillReadDto()
    {
        var handler = BuildHandler(nameof(ValidCommand_ReturnsSkillReadDto));

        var result = await handler.HandleAsync(ValidCommand());

        result.Should().NotBeNull();
        result.Name.Should().Be(".NET");
        result.Slug.Should().Be("dotnet");
        result.Category.Should().Be("Backend");
    }

    [Fact]
    public async Task ValidCommand_PersistsToDatabase()
    {
        var db = DbContextFactory.Create(nameof(ValidCommand_PersistsToDatabase));
        var handler = new CreateSkillCommandHandler(db, NullLogger<CreateSkillCommandHandler>.Instance);

        await handler.HandleAsync(ValidCommand());

        db.Skills.Should().ContainSingle(s => s.Slug == "dotnet");
    }

    [Fact]
    public async Task DuplicateSlug_ThrowsInvalidOperationException()
    {
        var dbName = nameof(DuplicateSlug_ThrowsInvalidOperationException);
        var handler = BuildHandler(dbName);
        await handler.HandleAsync(ValidCommand());

        var act = () => handler.HandleAsync(ValidCommand());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*dotnet*");
    }

    [Fact]
    public async Task Slug_IsTrimmedAndLowercased()
    {
        var handler = BuildHandler(nameof(Slug_IsTrimmedAndLowercased));

        var result = await handler.HandleAsync(ValidCommand(slug: "  DotNet  "));

        result.Slug.Should().Be("dotnet");
    }

    [Fact]
    public async Task ValidCommand_AssignsId()
    {
        var handler = BuildHandler(nameof(ValidCommand_AssignsId));

        var result = await handler.HandleAsync(ValidCommand());

        result.Id.Should().BeGreaterThan(0);
    }
}
