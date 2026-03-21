using Portfolio.Api.Services;

namespace Portfolio.Api.Features.Auth.Commands.Login;

public class LoginCommandHandler
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(ITokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    // Returns a signed JWT on success, null if credentials are invalid.
    // Password comparison is case-sensitive; username is case-insensitive.
    public Task<string?> HandleAsync(LoginCommand command)
    {
        var expectedUsername = _configuration["AdminCredentials:Username"];
        var expectedPassword = _configuration["AdminCredentials:Password"];

        var usernameMatch = string.Equals(command.Username, expectedUsername, StringComparison.OrdinalIgnoreCase);
        var passwordMatch = string.Equals(command.Password, expectedPassword, StringComparison.Ordinal);

        if (!usernameMatch || !passwordMatch)
            return Task.FromResult<string?>(null);

        var token = _tokenService.GenerateToken(command.Username);
        return Task.FromResult<string?>(token);
    }
}
