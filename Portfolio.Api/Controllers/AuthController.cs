using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Dtos.Auth;
using Portfolio.Api.Features.Auth.Commands.Login;
using Portfolio.Api.Filters;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly LoginCommandHandler _login;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public AuthController(LoginCommandHandler login, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _login = login;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Returns 200 if the current request carries a valid JWT cookie, 401 otherwise.
    /// Used by the frontend to check auth state on load.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me() => Ok();

    /// <summary>
    /// Authenticates the admin user and issues a JWT in an HttpOnly cookie.
    /// </summary>
    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilter<LoginRequestDto>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var token = await _login.HandleAsync(new LoginCommand(dto.Username, dto.Password));

        if (token is null)
            return Unauthorized();

        var expiryHours = int.Parse(_configuration["Jwt:ExpiryHours"]!);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            // Secure is required in production; relaxed in development to support HTTP.
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(expiryHours)
        });

        return Ok();
    }

    /// <summary>
    /// Clears the JWT cookie, ending the session.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt", new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict
        });

        return Ok();
    }
}
