using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Dtos.Auth;
using Portfolio.Api.Features.Auth.Commands.Login;
using Portfolio.Api.Filters;
using Portfolio.Api.Services;
using Portfolio.Api.Validators;
using System.Text;

namespace Portfolio.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtCookieAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    // Prevent clock skew from allowing expired tokens a grace period
                    ClockSkew = TimeSpan.Zero
                };

                // Read the token from the HttpOnly cookie instead of the Authorization header.
                // The browser sends the cookie automatically — the frontend never touches the token.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["jwt"];
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        // Auth feature dependencies
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
        services.AddScoped<ValidationFilter<LoginRequestDto>>();

        return services;
    }
}
