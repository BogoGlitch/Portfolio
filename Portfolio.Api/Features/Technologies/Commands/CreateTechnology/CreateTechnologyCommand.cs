using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Features.Technologies.Commands.CreateTechnology;

/// <summary>
/// Represents a request to create a new technology.
/// </summary>
public record CreateTechnologyCommand(CreateTechnologyDto Dto);
