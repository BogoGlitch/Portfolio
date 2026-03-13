using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Entities;
using System.Linq.Expressions;

namespace Portfolio.Api.Common.Projections;

public static class ProjectProjections
{
    public static Expression<Func<Project, ProjectReadDto>> ToDto()
    {
        return project => new ProjectReadDto
        {
            Name = project.Name,
            Slug = project.Slug,
            ShortDescription = project.ShortDescription,
            FullDescription = project.FullDescription,
            RepoUrl = project.RepoUrl,
            LiveUrl = project.LiveUrl,
            ImageUrl = project.ImageUrl,
            IsFeatured = project.IsFeatured
        };
    }
}
