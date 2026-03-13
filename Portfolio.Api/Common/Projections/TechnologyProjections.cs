using Portfolio.Api.Dtos;
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
            technology.LogoUrl,
            technology.DocumentationUrl,
            technology.IsFeatured,
            technology.DisplayOrder
        );
    }
}
