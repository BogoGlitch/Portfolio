using FluentAssertions;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Validators;

namespace Portfolio.Tests.Validators;

public class CreateTechnologyValidatorTests
{
    private readonly CreateTechnologyValidator _validator = new();

    private static CreateTechnologyDto ValidDto(
        string name = "React",
        string slug = "react",
        string description = "A JavaScript library.",
        string category = "Frontend",
        string? logoUrl = null,
        string? documentationUrl = null,
        int displayOrder = 0) =>
        new(name, slug, description, category, logoUrl, documentationUrl, false, displayOrder);

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

    [Fact]
    public void Name_ExceedsMaxLength_FailsValidation()
    {
        var result = _validator.Validate(ValidDto(name: new string('x', 101)));
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
    [InlineData("React")]           // uppercase
    [InlineData("my slug")]         // space
    [InlineData("my_slug")]         // underscore
    [InlineData("my.slug")]         // dot
    public void Slug_InvalidFormat_FailsValidation(string slug)
    {
        var result = _validator.Validate(ValidDto(slug: slug));
        result.Errors.Should().Contain(e => e.PropertyName == "Slug");
    }

    [Theory]
    [InlineData("react")]
    [InlineData("dot-net")]
    [InlineData("net10")]
    public void Slug_ValidFormat_PassesValidation(string slug)
    {
        var result = _validator.Validate(ValidDto(slug: slug));
        result.Errors.Should().NotContain(e => e.PropertyName == "Slug");
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
    [InlineData("/relative/path")]
    public void LogoUrl_InvalidUrl_FailsValidation(string url)
    {
        var result = _validator.Validate(ValidDto(logoUrl: url));
        result.Errors.Should().Contain(e => e.PropertyName == "LogoUrl");
    }

    [Theory]
    [InlineData("https://example.com/logo.png")]
    [InlineData("http://example.com/logo.png")]
    public void LogoUrl_ValidUrl_PassesValidation(string url)
    {
        var result = _validator.Validate(ValidDto(logoUrl: url));
        result.Errors.Should().NotContain(e => e.PropertyName == "LogoUrl");
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

    [Fact]
    public void ZeroDisplayOrder_PassesValidation()
    {
        var result = _validator.Validate(ValidDto(displayOrder: 0));
        result.Errors.Should().NotContain(e => e.PropertyName == "DisplayOrder");
    }
}
