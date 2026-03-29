using FluentAssertions;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Validators;

namespace Portfolio.Tests.Validators;

// UpdateTechnologyValidator uses identical rules to CreateTechnologyValidator.
// These tests confirm the rules are present on the separate class, since the two
// classes are independent and could diverge in the future.
public class UpdateTechnologyValidatorTests
{
    private readonly UpdateTechnologyValidator _validator = new();

    private static UpdateTechnologyDto ValidDto(
        string name = "React",
        string slug = "react",
        string description = "A JavaScript library.",
        string category = "Frontend",
        string discipline = "Frontend",
        string? logoUrl = null,
        string? documentationUrl = null,
        int displayOrder = 0) =>
        new(name, slug, description, category, discipline, logoUrl, documentationUrl, false, displayOrder);

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
    [InlineData("React")]        // uppercase
    [InlineData("my slug")]      // space
    [InlineData("my_slug")]      // underscore
    public void Slug_InvalidFormat_FailsValidation(string slug)
    {
        var result = _validator.Validate(ValidDto(slug: slug));
        result.Errors.Should().Contain(e => e.PropertyName == "Slug");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyDescription_FailsValidation(string description)
    {
        var result = _validator.Validate(ValidDto(description: description));
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyCategory_FailsValidation(string category)
    {
        var result = _validator.Validate(ValidDto(category: category));
        result.Errors.Should().Contain(e => e.PropertyName == "Category");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    public void LogoUrl_InvalidUrl_FailsValidation(string url)
    {
        var result = _validator.Validate(ValidDto(logoUrl: url));
        result.Errors.Should().Contain(e => e.PropertyName == "LogoUrl");
    }

    [Fact]
    public void LogoUrl_Null_PassesValidation()
    {
        var result = _validator.Validate(ValidDto(logoUrl: null));
        result.Errors.Should().NotContain(e => e.PropertyName == "LogoUrl");
    }

    [Fact]
    public void NegativeDisplayOrder_FailsValidation()
    {
        var result = _validator.Validate(ValidDto(displayOrder: -1));
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayOrder");
    }
}
