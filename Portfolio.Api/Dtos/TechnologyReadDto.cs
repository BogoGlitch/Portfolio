namespace Portfolio.Api.Dtos;

public record TechnologyReadDto(
    int Id,
    string Name,
    string Slug,
    string Description,
    string Category,
    string? LogoUrl,
    string? DocumentationUrl,
    bool IsFeatured,
    int DisplayOrder);
