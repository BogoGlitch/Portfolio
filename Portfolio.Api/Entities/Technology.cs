namespace Portfolio.Api.Entities;

public class Technology
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? DocumentationUrl { get; set; }

    public bool IsFeatured { get; set; } = false;

    public DateTime DateAddedUtc { get; set; }
}
