using Portfolio.Api.Data;

namespace Portfolio.Api.Features.Technologies.Commands.DeleteTechnology;

/// <summary>
/// Handles the DeleteTechnologyCommand. Returns true if the technology was found and deleted,
/// false if it did not exist — allowing the controller to produce a 404 without throwing.
/// </summary>
public class DeleteTechnologyCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<DeleteTechnologyCommandHandler> _logger;

    public DeleteTechnologyCommandHandler(AppDbContext db, ILogger<DeleteTechnologyCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(DeleteTechnologyCommand command, CancellationToken cancellationToken = default)
    {
        var technology = await _db.Technologies.FindAsync([command.Id], cancellationToken);

        if (technology is null)
        {
            return false;
        }

        _db.Technologies.Remove(technology);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted technology {TechnologyId}.", command.Id);

        return true;
    }
}
