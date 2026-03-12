namespace Portfolio.Api.Dtos;

public class ProjectDto
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public string FullDescription { get; set; } = null!;

    public string? RepoUrl { get; set; }

    public string? LiveUrl { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsFeatured { get; set; }
}
