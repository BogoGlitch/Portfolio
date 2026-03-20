using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Extensions;
using Portfolio.Api.Features.Projects.Commands.CreateProject;
using Portfolio.Api.Features.Projects.Commands.DeleteProject;
using Portfolio.Api.Features.Projects.Commands.UpdateProject;
using Portfolio.Api.Features.Projects.Queries.GetProjectBySlug;
using Portfolio.Api.Features.Projects.Queries.GetProjects;
using Portfolio.Api.Features.Technologies.Commands.CreateTechnology;
using Portfolio.Api.Features.Technologies.Commands.DeleteTechnology;
using Portfolio.Api.Features.Technologies.Commands.UpdateTechnology;
using Portfolio.Api.Features.Technologies.Queries.GetTechnologies;
using Portfolio.Api.Features.Technologies.Queries.GetTechnologyBySlug;
using Portfolio.Api.Filters;
using Portfolio.Api.Validators;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Replace the default .NET logger with Serilog.
// All ILogger<T> injections throughout the app continue to work unchanged —
// Serilog plugs in as the underlying provider behind the same interface.
builder.Host.UseSerilog(SerilogConfiguration.Configure);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Project query handlers
builder.Services.AddScoped<GetProjectsQueryHandler>();
builder.Services.AddScoped<GetProjectBySlugQueryHandler>();

// Project command handlers
builder.Services.AddScoped<CreateProjectCommandHandler>();
builder.Services.AddScoped<UpdateProjectCommandHandler>();
builder.Services.AddScoped<DeleteProjectCommandHandler>();

// Technology query handlers
builder.Services.AddScoped<GetTechnologiesQueryHandler>();
builder.Services.AddScoped<GetTechnologyBySlugQueryHandler>();

// Technology command handlers
builder.Services.AddScoped<CreateTechnologyCommandHandler>();
builder.Services.AddScoped<UpdateTechnologyCommandHandler>();
builder.Services.AddScoped<DeleteTechnologyCommandHandler>();

// Validators — one per writable DTO
builder.Services.AddScoped<IValidator<CreateProjectDto>, CreateProjectValidator>();
builder.Services.AddScoped<IValidator<UpdateProjectDto>, UpdateProjectValidator>();
builder.Services.AddScoped<IValidator<CreateTechnologyDto>, CreateTechnologyValidator>();
builder.Services.AddScoped<IValidator<UpdateTechnologyDto>, UpdateTechnologyValidator>();

// Validation filters — registered so ServiceFilter can resolve them from DI
builder.Services.AddScoped<ValidationFilter<CreateProjectDto>>();
builder.Services.AddScoped<ValidationFilter<UpdateProjectDto>>();
builder.Services.AddScoped<ValidationFilter<CreateTechnologyDto>>();
builder.Services.AddScoped<ValidationFilter<UpdateTechnologyDto>>();

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
app.MapHealthCheckEndpoints();
app.MapControllers();

app.Run();
