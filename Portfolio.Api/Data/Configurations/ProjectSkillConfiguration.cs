using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Data.Configurations;

public class ProjectSkillConfiguration : IEntityTypeConfiguration<ProjectSkill>
{
    public void Configure(EntityTypeBuilder<ProjectSkill> builder)
    {
        builder.ToTable("ProjectSkills");

        builder.HasKey(ps => new { ps.ProjectId, ps.SkillId });

        builder
            .HasOne(ps => ps.Project)
            .WithMany(p => p.ProjectSkills)
            .HasForeignKey(ps => ps.ProjectId);

        builder
            .HasOne(ps => ps.Skill)
            .WithMany(s => s.ProjectSkills)
            .HasForeignKey(ps => ps.SkillId);
    }
}
