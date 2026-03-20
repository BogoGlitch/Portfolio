using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Filters;
using Portfolio.Api.Interfaces;
using Portfolio.Api.Services;
using Portfolio.Api.Validators;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITechnologyService, TechnologyService>();
builder.Services.AddScoped<IProjectService, ProjectService>();

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
app.MapControllers();

app.Run();
