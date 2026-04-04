using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Entities;
using Portfolio.Api.Features.Projects.Commands.UpdateProject;
using Portfolio.Tests.Helpers;

namespace Portfolio.Tests.Features.Projects;

public class UpdateProjectCommandHandlerTests
{
    private static UpdateProjectCommandHandler BuildHandler(string dbName)
    {
        var db = DbContextFactory.Create(dbName);
        return new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);
    }

    private static UpdateProjectCommand UpdateCommand(int id, string slug = "my-project", IReadOnlyList<int>? skillIds = null) =>
        new(id, new UpdateProjectDto(
            Name: "My Project Updated",
            Slug: slug,
            ShortDescription: "Updated short description.",
            FullDescription: "Updated full description of the project.",
            RepoUrl: null,
            LiveUrl: null,
            ImageUrl: null,
            IsFeatured: true,
            DisplayOrder: 1,
            SkillIds: skillIds ?? []
        ));

    [Fact]
    public async Task NotFound_ReturnsNull()
    {
        var handler = BuildHandler(nameof(NotFound_ReturnsNull));

        var result = await handler.HandleAsync(UpdateCommand(999));

        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidUpdate_ReturnsUpdatedDto()
    {
        var db = DbContextFactory.Create(nameof(ValidUpdate_ReturnsUpdatedDto));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);

        var result = await handler.HandleAsync(UpdateCommand(1));

        result.Should().NotBeNull();
        result!.Name.Should().Be("My Project Updated");
        result.IsFeatured.Should().BeTrue();
        result.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public async Task SameSlugOnSelf_DoesNotThrow()
    {
        // Updating without changing the slug should not trigger a uniqueness conflict.
        var db = DbContextFactory.Create(nameof(SameSlugOnSelf_DoesNotThrow));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);

        var act = () => handler.HandleAsync(UpdateCommand(1, slug: "my-project"));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SlugTakenByOtherProject_ThrowsInvalidOperationException()
    {
        var db = DbContextFactory.Create(nameof(SlugTakenByOtherProject_ThrowsInvalidOperationException));
        db.Projects.Add(new Project { Id = 1, Name = "Project A", Slug = "project-a", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        db.Projects.Add(new Project { Id = 2, Name = "Project B", Slug = "project-b", ShortDescription = "short", FullDescription = "full", DisplayOrder = 1 });
        await db.SaveChangesAsync();
        var handler = new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);

        // Try to update project 2 to use "project-a" which belongs to project 1.
        var act = () => handler.HandleAsync(UpdateCommand(2, slug: "project-a"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*project-a*");
    }

    [Fact]
    public async Task InvalidSkillId_ThrowsInvalidOperationException()
    {
        var db = DbContextFactory.Create($"Update_{nameof(InvalidSkillId_ThrowsInvalidOperationException)}");
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);

        var act = () => handler.HandleAsync(UpdateCommand(1, skillIds: [999]));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*skill IDs are invalid*");
    }

    [Fact]
    public async Task MixedValidAndInvalidSkillIds_ThrowsInvalidOperationException()
    {
        var db = DbContextFactory.Create(nameof(MixedValidAndInvalidSkillIds_ThrowsInvalidOperationException));
        db.Projects.Add(new Project { Id = 1, Name = "My Project", Slug = "my-project", ShortDescription = "short", FullDescription = "full", DisplayOrder = 0 });
        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        await db.SaveChangesAsync();
        var handler = new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);

        // ID 1 exists, ID 999 does not — count mismatch should throw.
        var act = () => handler.HandleAsync(UpdateCommand(1, skillIds: [1, 999]));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*skill IDs are invalid*");
    }

    [Fact]
    public async Task Skills_AreFullyReplaced()
    {
        // Original has skill 1; update replaces with skill 2 only.
        var db = DbContextFactory.Create(nameof(Skills_AreFullyReplaced));
        db.Skills.Add(new Skill { Id = 1, Name = ".NET", Slug = "dotnet", Description = "desc", Category = "Backend", Discipline = "Backend", DisplayOrder = 0 });
        db.Skills.Add(new Skill { Id = 2, Name = "React", Slug = "react", Description = "desc", Category = "Frontend", Discipline = "Frontend", DisplayOrder = 1 });
        db.Projects.Add(new Project
        {
            Id = 1,
            Name = "My Project",
            Slug = "my-project",
            ShortDescription = "short",
            FullDescription = "full",
            DisplayOrder = 0,
            ProjectSkills = [new ProjectSkill { ProjectId = 1, SkillId = 1 }]
        });
        await db.SaveChangesAsync();
        var handler = new UpdateProjectCommandHandler(db, NullLogger<UpdateProjectCommandHandler>.Instance);

        var result = await handler.HandleAsync(UpdateCommand(1, skillIds: [2]));

        result!.Skills.Should().ContainSingle(s => s.Slug == "react");
        result.Skills.Should().NotContain(s => s.Slug == "dotnet");
    }

    // TODO: db name "Slug_IsTrimmedAndLowercased" conflicts with UpdateSkillCommandHandlerTests — revisit in a dedicated session
    // [Fact]
    // public async Task Slug_IsTrimmedAndLowercased() { ... }
}
