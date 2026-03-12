using Portfolio.Api.Dtos;
using Portfolio.Api.Entities;
using System.Linq.Expressions;

namespace Portfolio.Api.Common.Projections;

public static class ProjectProjections
{
    public static Expression<Func<Project, ProjectDto>> ToDto()
    {
        return project => new ProjectDto
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
