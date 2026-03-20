using Portfolio.Api.Data;

namespace Portfolio.Api.Features.Projects.Commands.DeleteProject;

/// <summary>
/// Handles the DeleteProjectCommand. Returns true if the project was found and deleted,
/// false if it did not exist — allowing the controller to produce a 404 without throwing.
/// </summary>
public class DeleteProjectCommandHandler
{
    private readonly AppDbContext _db;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(AppDbContext db, ILogger<DeleteProjectCommandHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await _db.Projects.FindAsync([command.Id], cancellationToken);

        if (project is null)
        {
            return false;
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted project {ProjectId}.", command.Id);

        return true;
    }
}
