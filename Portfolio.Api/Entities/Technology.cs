namespace Portfolio.Api.Entities;

public class Technology
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Discipline { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public string? DocumentationUrl { get; set; }

    public string? FullStory { get; set; }

    public bool IsFeatured { get; set; } = false;

    public int DisplayOrder { get; set; }

    public DateTime DateAddedUtc { get; set; }

    public ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = [];
}
