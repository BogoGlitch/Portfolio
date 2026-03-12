using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Api.Entities;

namespace Portfolio.Api.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table
        builder.ToTable("Projects");

        // Key(s)
        builder.HasKey(p => p.Id);

        // Index(es)
        builder.HasIndex(p => p.Slug)
            .IsUnique();

        //Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.ShortDescription)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.FullDescription)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(p => p.RepoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.LiveUrl)
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.IsFeatured)
            .IsRequired();

        builder.Property(p => p.DateCreatedUtc)
            .IsRequired();

    }
}
