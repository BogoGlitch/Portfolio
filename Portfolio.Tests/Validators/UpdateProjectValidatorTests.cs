using FluentAssertions;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Validators;

namespace Portfolio.Tests.Validators;

// UpdateProjectValidator uses identical rules to CreateProjectValidator.
// These tests confirm the rules are present on the separate class, since the two
// classes are independent and could diverge in the future.
public class UpdateProjectValidatorTests
{
    private readonly UpdateProjectValidator _validator = new();

    private static UpdateProjectDto ValidDto(
        string name = "My Project",
        string slug = "my-project",
        string shortDescription = "A short description.",
        string fullDescription = "A full description of the project.",
        string? repoUrl = null,
        string? liveUrl = null,
        int displayOrder = 0) =>
        new(name, slug, shortDescription, fullDescription, repoUrl, liveUrl, null, false, displayOrder, []);

    [Fact]
    public void ValidDto_PassesValidation()
    {
        var result = _validator.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyName_FailsValidation(string name)
    {
        var result = _validator.Validate(ValidDto(name: name));
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptySlug_FailsValidation(string slug)
    {
        var result = _validator.Validate(ValidDto(slug: slug));
        result.Errors.Should().Contain(e => e.PropertyName == "Slug");
    }

    [Theory]
    [InlineData("My-Project")]    // uppercase
    [InlineData("my project")]    // space
    [InlineData("my_project")]    // underscore
    public void Slug_InvalidFormat_FailsValidation(string slug)
    {
        var result = _validator.Validate(ValidDto(slug: slug));
        result.Errors.Should().Contain(e => e.PropertyName == "Slug");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyShortDescription_FailsValidation(string value)
    {
        var result = _validator.Validate(ValidDto(shortDescription: value));
        result.Errors.Should().Contain(e => e.PropertyName == "ShortDescription");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyFullDescription_FailsValidation(string value)
    {
        var result = _validator.Validate(ValidDto(fullDescription: value));
        result.Errors.Should().Contain(e => e.PropertyName == "FullDescription");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/repo")]
    public void RepoUrl_InvalidUrl_FailsValidation(string url)
    {
        var result = _validator.Validate(ValidDto(repoUrl: url));
        result.Errors.Should().Contain(e => e.PropertyName == "RepoUrl");
    }

    [Fact]
    public void RepoUrl_Null_PassesValidation()
    {
        var result = _validator.Validate(ValidDto(repoUrl: null));
        result.Errors.Should().NotContain(e => e.PropertyName == "RepoUrl");
    }

    [Fact]
    public void NegativeDisplayOrder_FailsValidation()
    {
        var result = _validator.Validate(ValidDto(displayOrder: -1));
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayOrder");
    }
}
