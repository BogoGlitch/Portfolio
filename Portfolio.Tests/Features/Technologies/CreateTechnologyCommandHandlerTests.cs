using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Technologies.Commands.CreateTechnology;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Technologies;

public class CreateTechnologyCommandHandlerTests
{
    private static CreateTechnologyCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new CreateTechnologyCommandHandler(db, NullLogger<CreateTechnologyCommandHandler>.Instance);
    }

    private static CreateTechnologyCommand ValidCommand(string slug = "dotnet") =>
        new(new CreateTechnologyDto(
            Name: ".NET",
            Slug: slug,
            Description: "A cross-platform framework.",
            Category: "Backend",
            Discipline: "Backend",
            LogoUrl: null,
            DocumentationUrl: null,
            IsFeatured: false,
            DisplayOrder: 0
        ));

    [Fact]
    public async Task ValidCommand_ReturnsTechnologyReadDto()
    {
        var handler = BuildHandler(nameof(ValidCommand_ReturnsTechnologyReadDto));

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
        var handler = new CreateTechnologyCommandHandler(db, NullLogger<CreateTechnologyCommandHandler>.Instance);

        await handler.HandleAsync(ValidCommand());

        db.Technologies.Should().ContainSingle(t => t.Slug == "dotnet");
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
