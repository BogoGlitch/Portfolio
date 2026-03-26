namespace Portfolio.Api.Dtos.Technologies;

public record UpdateTechnologyDto
(
    string Name,
    string Slug,
    string Description,
    string Category,
    string Discipline,
    string? LogoUrl,
    string? DocumentationUrl,
    bool IsFeatured,
    int DisplayOrder
);