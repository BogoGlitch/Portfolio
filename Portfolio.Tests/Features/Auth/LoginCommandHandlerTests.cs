using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Portfolio.Api.Features.Auth.Commands.Login;
using Portfolio.Api.Services;

namespace Portfolio.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();

    private LoginCommandHandler BuildHandler(string username = "admin", string password = "secret")
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminCredentials:Username"] = username,
                ["AdminCredentials:Password"] = password,
            })
            .Build();

        return new LoginCommandHandler(_tokenService, config);
    }

    [Fact]
    public async Task ValidCredentials_ReturnsToken()
    {
        _tokenService.GenerateToken("admin").Returns("signed-jwt");
        var handler = BuildHandler();

        var result = await handler.HandleAsync(new LoginCommand("admin", "secret"));

        result.Should().Be("signed-jwt");
    }

    [Fact]
    public async Task WrongPassword_ReturnsNull()
    {
        var handler = BuildHandler();

        var result = await handler.HandleAsync(new LoginCommand("admin", "wrong"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task WrongUsername_ReturnsNull()
    {
        var handler = BuildHandler();

        var result = await handler.HandleAsync(new LoginCommand("notadmin", "secret"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task Username_IsCaseInsensitive()
    {
        _tokenService.GenerateToken(Arg.Any<string>()).Returns("token");
        var handler = BuildHandler();

        var result = await handler.HandleAsync(new LoginCommand("ADMIN", "secret"));

        result.Should().Be("token");
    }

    [Fact]
    public async Task Password_IsCaseSensitive()
    {
        var handler = BuildHandler();

        var result = await handler.HandleAsync(new LoginCommand("admin", "SECRET"));

        result.Should().BeNull();
    }
}
