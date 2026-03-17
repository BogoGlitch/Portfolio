namespace Portfolio.Api.Dtos.Projects;

public record UpdateProjectDto
(
    string Name,
    string Slug,
    string ShortDescription,
    string FullDescription,
    string? RepoUrl,
    string? LiveUrl,
    string? ImageUrl,
    bool IsFeatured,
    int DisplayOrder,
    IReadOnlyList<int> TechnologyIds
);
