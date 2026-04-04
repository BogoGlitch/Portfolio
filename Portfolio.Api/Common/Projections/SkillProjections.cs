using Portfolio.Api.Dtos.Projects;
using Portfolio.Api.Dtos.Skills;
using Portfolio.Api.Entities;
using System.Linq.Expressions;

namespace Portfolio.Api.Common.Projections;

public static class SkillProjections
{
    public static Expression<Func<Skill, SkillReadDto>> ToDto()
    {
        return skill => new SkillReadDto
        (
            skill.Id,
            skill.Name,
            skill.Slug,
            skill.Description,
            skill.Category,
            skill.Discipline,
            skill.LogoUrl,
            skill.DocumentationUrl,
            skill.FullStory,
            skill.IsFeatured,
            skill.DisplayOrder,
            skill.ProjectSkills
                .OrderBy(ps => ps.Project.DisplayOrder)
                .ThenBy(ps => ps.Project.Name)
                .Select(ps => new ProjectSummaryDto
                    (
                        ps.Project.Id,
                        ps.Project.Name,
                        ps.Project.Slug
                    )
                )
                .ToList()
        );
    }
}
