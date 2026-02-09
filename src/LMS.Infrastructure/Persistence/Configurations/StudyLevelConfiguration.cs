using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for StudyLevel
/// </summary>
public sealed class StudyLevelConfiguration : IEntityTypeConfiguration<StudyLevel>
{
    public void Configure(EntityTypeBuilder<StudyLevel> builder)
    {
        builder.ToTable("StudyLevel");

        builder.HasKey(sl => sl.Id);

        builder.Property(sl => sl.Id)
            .ValueGeneratedNever();

        builder.Property(sl => sl.NameAr)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sl => sl.NameEn)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sl => sl.DisplayOrder)
            .IsRequired();

        // Indexes
        builder.HasIndex(sl => sl.DisplayOrder)
            .HasDatabaseName("IX_StudyLevel_DisplayOrder");

        // Note: Seed data moved to DbInitializer.cs for runtime seeding
    }
}
