using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Track
/// </summary>
public sealed class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("Track");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.NameAr)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.NameEn)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.DisplayOrder)
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.DisplayOrder)
            .HasDatabaseName("IX_Track_DisplayOrder");

        // Note: Seed data moved to DbInitializer.cs for runtime seeding
    }
}
