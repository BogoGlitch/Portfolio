using FluentAssertions;
using Portfolio.Api.Dtos.Auth;
using Portfolio.Api.Validators;

namespace Portfolio.Tests.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void ValidDto_PassesValidation()
    {
        var dto = new LoginRequestDto { Username = "admin", Password = "secret" };
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyUsername_FailsValidation(string username)
    {
        var dto = new LoginRequestDto { Username = username, Password = "secret" };
        var result = _validator.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyPassword_FailsValidation(string password)
    {
        var dto = new LoginRequestDto { Username = "admin", Password = password };
        var result = _validator.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}
