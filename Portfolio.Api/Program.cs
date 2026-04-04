using Azure.Identity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Auth;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Skills;
using Portfolio.Api.Extensions;
using Portfolio.Api.Features.Projects.Commands.CreateProject;
using Portfolio.Api.Features.Projects.Commands.DeleteProject;
using Portfolio.Api.Features.Projects.Commands.UpdateProject;
using Portfolio.Api.Features.Projects.Queries.GetProjectBySlug;
using Portfolio.Api.Features.Projects.Queries.GetProjects;
using Portfolio.Api.Features.Skills.Commands.CreateSkill;
using Portfolio.Api.Features.Skills.Commands.DeleteSkill;
using Portfolio.Api.Features.Skills.Commands.UpdateSkill;
using Portfolio.Api.Features.Skills.Queries.GetSkills;
using Portfolio.Api.Features.Skills.Queries.GetSkillBySlug;
using Portfolio.Api.Filters;
using Portfolio.Api.Validators;
using Portfolio.Api.Features.Auth.Commands.Login;
using Portfolio.Api.Services;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// In production, pull secrets from Azure Key Vault using the App Service's managed identity.
// DefaultAzureCredential automatically uses the managed identity when running in Azure,
// and falls back to developer credentials (VS, CLI) when running locally.
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri(builder.Configuration["KeyVaultUri"]!);
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

// Replace the default .NET logger with Serilog.
// All ILogger<T> injections throughout the app continue to work unchanged —
// Serilog plugs in as the underlying provider behind the same interface.
builder.Host.UseSerilog(SerilogConfiguration.Configure);

if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
    builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

builder.Services.AddJwtCookieAuthentication(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Auth handlers
builder.Services.AddScoped<LoginCommandHandler>();

// Project query handlers
builder.Services.AddScoped<GetProjectsQueryHandler>();
builder.Services.AddScoped<GetProjectBySlugQueryHandler>();

// Project command handlers
builder.Services.AddScoped<CreateProjectCommandHandler>();
builder.Services.AddScoped<UpdateProjectCommandHandler>();
builder.Services.AddScoped<DeleteProjectCommandHandler>();

// Skill query handlers
builder.Services.AddScoped<GetSkillsQueryHandler>();
builder.Services.AddScoped<GetSkillBySlugQueryHandler>();

// Skill command handlers
builder.Services.AddScoped<CreateSkillCommandHandler>();
builder.Services.AddScoped<UpdateSkillCommandHandler>();
builder.Services.AddScoped<DeleteSkillCommandHandler>();

// Validators — one per writable DTO
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<CreateProjectDto>, CreateProjectValidator>();
builder.Services.AddScoped<IValidator<UpdateProjectDto>, UpdateProjectValidator>();
builder.Services.AddScoped<IValidator<CreateSkillDto>, CreateSkillValidator>();
builder.Services.AddScoped<IValidator<UpdateSkillDto>, UpdateSkillValidator>();

// Validation filters — registered so ServiceFilter can resolve them from DI
builder.Services.AddScoped<ValidationFilter<LoginRequestDto>>();
builder.Services.AddScoped<ValidationFilter<CreateProjectDto>>();
builder.Services.AddScoped<ValidationFilter<UpdateProjectDto>>();
builder.Services.AddScoped<ValidationFilter<CreateSkillDto>>();
builder.Services.AddScoped<ValidationFilter<UpdateSkillDto>>();

// Keep Azure SQL Basic from going idle between requests
builder.Services.AddHostedService<DatabaseKeepAliveService>();

// Pre-compile EF queries at startup so the first real request is fast
builder.Services.AddHostedService<QueryWarmupService>();

// Health checks — /health (full) and /health/live (liveness only)
// AddDbContextCheck runs a test query against AppDbContext to confirm the DB is reachable.
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

// Problem Details — standardises all error responses (400, 404, 500, etc.)
// to the RFC 7807 shape: { type, title, status, detail, traceId }
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// UseExceptionHandler() catches unhandled exceptions and formats them as Problem Details.
// UseStatusCodePages() turns raw 404/405 responses into Problem Details too.
app.UseExceptionHandler();
app.UseStatusCodePages();

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthCheckEndpoints();
app.MapControllers();

app.Run();
