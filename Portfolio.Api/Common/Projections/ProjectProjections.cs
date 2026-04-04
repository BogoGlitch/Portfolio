using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Skills;
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
            project.ProjectSkills
                .OrderBy(ps => ps.Skill.DisplayOrder)
                .ThenBy(ps => ps.Skill.Name)
                .Select(ps => new SkillSummaryDto
                    (
                        ps.Skill.Id,
                        ps.Skill.Name,
                        ps.Skill.Slug
                    )
                )
                .ToList()

        );
    }
}
