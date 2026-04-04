using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Skills;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Features.Skills.Commands.CreateSkill;

/// <summary>
/// Handles the CreateSkillCommand. Validates slug uniqueness,
/// persists the new skill, then re-fetches it for the response.
/// </summary>
public class CreateSkillCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateSkillCommandHandler> _logger;

    public CreateSkillCommandHandler(AppDbContext db, ILogger<CreateSkillCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<SkillReadDto> HandleAsync(CreateSkillCommand command, CancellationToken cancellationToken = default)
    {
        var dto = command.Dto;
        var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();

        var slugExists = await _db.Skills
            .AnyAsync(s => s.Slug == normalizedSlug, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A skill with slug '{normalizedSlug}' already exists.");
        }

        var skill = new Skill
        {
            Name = dto.Name,
            Slug = normalizedSlug,
            Description = dto.Description,
            Category = dto.Category,
            Discipline = dto.Discipline,
            LogoUrl = dto.LogoUrl,
            DocumentationUrl = dto.DocumentationUrl,
            FullStory = dto.FullStory,
            IsFeatured = dto.IsFeatured,
            DisplayOrder = dto.DisplayOrder,
            DateAddedUtc = DateTime.UtcNow
        };

        _db.Skills.Add(skill);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created skill {SkillId} with slug {SkillSlug}.", skill.Id, skill.Slug);

        // Re-fetch with AsNoTracking so the projection runs cleanly against the database.
        return await _db.Skills
            .AsNoTracking()
            .Where(s => s.Id == skill.Id)
            .Select(SkillProjections.ToDto())
            .FirstAsync(cancellationToken);
    }
}
