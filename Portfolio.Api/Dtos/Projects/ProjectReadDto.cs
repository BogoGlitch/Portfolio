using Portfolio.Api.Dtos.Technologies;

namespace Portfolio.Api.Dtos.Projects;

public record ProjectReadDto
(
    int Id,
    string Name,
    string Slug,
    string ShortDescription,
    string FullDescription,
    string? RepoUrl,
    string? LiveUrl,
    string? ImageUrl,
    bool IsFeatured,
    int DisplayOrder,
    IReadOnlyList<TechnologySummaryDto> Technologies
);