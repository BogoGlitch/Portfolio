namespace Portfolio.Api.Dtos.Skills;

public record UpdateSkillDto
(
    string Name,
    string Slug,
    string Description,
    string Category,
    string Discipline,
    string? LogoUrl,
    string? DocumentationUrl,
    string? FullStory,
    bool IsFeatured,
    int DisplayOrder
);
