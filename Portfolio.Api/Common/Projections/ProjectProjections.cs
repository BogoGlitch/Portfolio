using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Entities;
using System.Linq.Expressions;

namespace Portfolio.Api.Common.Projections;

public static class ProjectProjections
{
    public static Expression<Func<Project, ProjectReadDto>> ToDto()
    {
        return project => new ProjectReadDto
        (
            project.Id,
            project.Name,
            project.Slug,
            project.ShortDescription,
            project.FullDescription,
            project.RepoUrl,
            project.LiveUrl,
            project.ImageUrl,
            project.IsFeatured,
            project.DisplayOrder,
            project.ProjectTechnologies
                .OrderBy(pt => pt.Technology.DisplayOrder)
                .ThenBy(pt => pt.Technology.Name)
                .Select(pt => new TechnologySummaryDto
                    (
                        pt.Technology.Id,
                        pt.Technology.Name,
                        pt.Technology.Slug
                    )
                )
                .ToList()

        );
    }
}
