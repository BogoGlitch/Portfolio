using Portfolio.Api.Dtos;
using Portfolio.Api.Entities;
using System.Linq.Expressions;

namespace Portfolio.Api.Common.Projections;

public static class TechnologyProjections
{
    public static readonly Expression<Func<Technology, TechnologyDto>> ToDto = technology => new TechnologyDto
    {
        Name = technology.Name,
        Slug = technology.Slug,
        Description = technology.Description,
        Category = technology.Category,
        LogoUrl = technology.LogoUrl,
        DocumentationUrl = technology.DocumentationUrl,
        IsFeatured = technology.IsFeatured
    };
}
