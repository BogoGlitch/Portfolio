using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Features.Technologies.Commands.UpdateTechnology;

/// <summary>
/// Represents a request to update an existing technology by its ID.
/// </summary>
public record UpdateTechnologyCommand(int Id, UpdateTechnologyDto Dto);
