using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Common.Projections;
using Portfolio.Api.Data;
using Portfolio.Api.Dtos.Skills;

namespace Portfolio.Api.Features.Skills.Commands.UpdateSkill;

/// <summary>
/// Handles the UpdateSkillCommand. Returns null if the skill does not exist,
/// allowing the controller to produce a 404 without throwing an exception.
/// </summary>
public class UpdateSkillCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<UpdateSkillCommandHandler> _logger;

    public UpdateSkillCommandHandler(AppDbContext db, ILogger<UpdateSkillCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<SkillReadDto?> HandleAsync(UpdateSkillCommand command, CancellationToken cancellationToken = default)
    {
        var skill = await _db.Skills.FindAsync([command.Id], cancellationToken);

        if (skill is null)
        {
            return null;
        }

        var dto = command.Dto;
        var normalizedSlug = dto.Slug.Trim().ToLowerInvariant();

        // Exclude the current skill from the uniqueness check so that updating
        // without changing the slug does not incorrectly trigger a conflict.
        var slugExists = await _db.Skills
            .AnyAsync(s => s.Slug == normalizedSlug && s.Id != command.Id, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A skill with slug '{normalizedSlug}' already exists.");
        }

        skill.Name = dto.Name;
        skill.Slug = normalizedSlug;
        skill.Description = dto.Description;
        skill.Category = dto.Category;
        skill.Discipline = dto.Discipline;
        skill.LogoUrl = dto.LogoUrl;
        skill.DocumentationUrl = dto.DocumentationUrl;
        skill.FullStory = dto.FullStory;
        skill.IsFeatured = dto.IsFeatured;
        skill.DisplayOrder = dto.DisplayOrder;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated skill {SkillId}.", skill.Id);

        return await _db.Skills
            .AsNoTracking()
            .Where(s => s.Id == skill.Id)
            .Select(SkillProjections.ToDto())
            .FirstAsync(cancellationToken);
    }
}
