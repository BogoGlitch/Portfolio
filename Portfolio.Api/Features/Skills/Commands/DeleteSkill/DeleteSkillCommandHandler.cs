using Portfolio.Api.Data;

namespace Portfolio.Api.Features.Skills.Commands.DeleteSkill;

/// <summary>
/// Handles the DeleteSkillCommand. Returns true if the skill was found and deleted,
/// false if it did not exist — allowing the controller to produce a 404 without throwing.
/// </summary>
public class DeleteSkillCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<DeleteSkillCommandHandler> _logger;

    public DeleteSkillCommandHandler(AppDbContext db, ILogger<DeleteSkillCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(DeleteSkillCommand command, CancellationToken cancellationToken = default)
    {
        var skill = await _db.Skills.FindAsync([command.Id], cancellationToken);

        if (skill is null)
        {
            return false;
        }

        _db.Skills.Remove(skill);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted skill {SkillId}.", command.Id);

        return true;
    }
}
