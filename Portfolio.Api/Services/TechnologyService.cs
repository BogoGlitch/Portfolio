using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Entities;
using Portfolio.Api.Interfaces;

namespace Portfolio.Api.Services;

public class TechnologyService : ITechnologyService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TechnologyService> _logger;

    public TechnologyService(AppDbContext context, ILogger<TechnologyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TechnologyReadDto>> GetTechnologiesAsync()
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Fetching all technologies.");
        }

        return await _context.Technologies
            .AsNoTracking()
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.Name)
            .Select(TechnologyProjections.ToDto())
            .ToListAsync();
    }

    public async Task<TechnologyReadDto?> GetTechnologyBySlugAsync(string slug)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Fetching technology by slug: {Slug}", slug);
        }

        slug = slug.Trim().ToLowerInvariant();

        return await _context.Technologies
            .AsNoTracking()
            .Where(t => t.Slug == slug)
            .Select(TechnologyProjections.ToDto())
            .FirstOrDefaultAsync();
    }

    public async Task<TechnologyReadDto> CreateTechnologyAsync(CreateTechnologyDto createTechnologyDto)
    {
        var normalizedSlug = createTechnologyDto.Slug.Trim().ToLowerInvariant();

        var slugExists = await _context.Technologies
            .AnyAsync(t => t.Slug == normalizedSlug);

        if (slugExists)
        {
            throw new InvalidOperationException($"A technology with slug '{normalizedSlug}' already exists.");
        }
        var technology = new Technology
        {
            Name = createTechnologyDto.Name,
            Slug = normalizedSlug,
            Description = createTechnologyDto.Description,
            Category = createTechnologyDto.Category,
            LogoUrl = createTechnologyDto.LogoUrl,
            DocumentationUrl = createTechnologyDto.DocumentationUrl,
            IsFeatured = createTechnologyDto.IsFeatured,
            DisplayOrder = createTechnologyDto.DisplayOrder
        };

        _context.Technologies.Add(technology);
        await _context.SaveChangesAsync();

        return TechnologyProjections.ToDto().Compile()(technology);
    }

    public async Task<TechnologyReadDto?> UpdateTechnologyAsync(int id, UpdateTechnologyDto updateTechnologyDto)
    {
        var technology = await _context.Technologies.FindAsync(id);

        if (technology == null)
        {
            return null;
        }

        var normalizedSlug = updateTechnologyDto.Slug.Trim().ToLowerInvariant();

        var slugExists = await _context.Technologies
            .AnyAsync(p => p.Slug == normalizedSlug && p.Id != id);

        if (slugExists)
        {
            throw new InvalidOperationException($"A technology with slug '{normalizedSlug}' already exists.");
        }

        technology.Name = updateTechnologyDto.Name;
        technology.Slug = normalizedSlug;
        technology.Description = updateTechnologyDto.Description;
        technology.Category = updateTechnologyDto.Category;
        technology.LogoUrl = updateTechnologyDto.LogoUrl;
        technology.DocumentationUrl = updateTechnologyDto.DocumentationUrl;
        technology.IsFeatured = updateTechnologyDto.IsFeatured;
        technology.DisplayOrder = updateTechnologyDto.DisplayOrder;

        await _context.SaveChangesAsync();

        return TechnologyProjections.ToDto().Compile()(technology);

    }

    public async Task<bool> DeleteTechnologyAsync(int id)
    {
        var technology = await _context.Technologies.FindAsync(id);

        if (technology is null)
        {
            return false;
        }

        _context.Technologies.Remove(technology);
        await _context.SaveChangesAsync();

        return true;

    }
}
