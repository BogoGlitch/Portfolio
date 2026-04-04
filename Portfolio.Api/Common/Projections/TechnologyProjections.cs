using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Technologies;
using Portfolio.Api.Entities;
using System.Linq.Expressions;

namespace Portfolio.Api.Common.Projections;

public static class TechnologyProjections
{
    public static Expression<Func<Technology, TechnologyReadDto>> ToDto()
    {
        return technology => new TechnologyReadDto
        (
            technology.Id,
            technology.Name,
            technology.Slug,
            technology.Description,
            technology.Category,
            technology.Discipline,
            technology.LogoUrl,
            technology.DocumentationUrl,
            technology.FullStory,
            technology.IsFeatured,
            technology.DisplayOrder,
            technology.ProjectTechnologies
                .OrderBy(pt => pt.Project.DisplayOrder)
                .ThenBy(pt => pt.Project.Name)
                .Select(pt => new ProjectSummaryDto
                    (
                        pt.Project.Id,
                        pt.Project.Name,
                        pt.Project.Slug
                    )
                )
                .ToList()
        );
    }
}
