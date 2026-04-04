using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Dtos.Technologies;

public record TechnologyReadDto
(
    int Id,
    string Name,
    string Slug,
    string Description,
    string Category,
    string Discipline,
    string? LogoUrl,
    string? DocumentationUrl,
    string? FullStory,
    bool IsFeatured,
    int DisplayOrder,
    IReadOnlyList<ProjectSummaryDto> Projects
 );