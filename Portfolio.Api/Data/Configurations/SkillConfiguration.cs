using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Data.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        // Table
        builder.ToTable("Skills");

        // Key(s)
        builder.HasKey(t => t.Id);

        // Index(es)
        builder.HasIndex(t => t.Slug)
            .IsUnique();

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Discipline)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.LogoUrl)
            .HasMaxLength(500);

        builder.Property(t => t.DocumentationUrl)
            .HasMaxLength(500);

        builder.Property(t => t.FullStory)
            .HasMaxLength(4000);

        builder.Property(t => t.IsFeatured)
            .IsRequired();

        builder.Property(t => t.DisplayOrder)
            .IsRequired();

        builder.Property(t => t.DateAddedUtc)
            .IsRequired();

    }
}
