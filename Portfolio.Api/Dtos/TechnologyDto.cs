namespace Portfolio.Api.Dtos;

public class TechnologyDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? DocumentationUrl { get; set; }
    public bool IsFeatured { get; set; }
}
