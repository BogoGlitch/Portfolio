using Portfolio.Api.Dtos.Projects;

namespace Portfolio.Api.Dtos.Skills;

public record SkillReadDto
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
