using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for StudyLevelTrack
/// </summary>
public sealed class StudyLevelTrackConfiguration : IEntityTypeConfiguration<StudyLevelTrack>
{
    public void Configure(EntityTypeBuilder<StudyLevelTrack> builder)
    {
        builder.ToTable("StudyLevelTrack");

        builder.HasKey(slt => slt.Id);

        builder.Property(slt => slt.Id)
            .ValueGeneratedNever();

        builder.Property(slt => slt.StudyLevelId)
            .IsRequired();

        builder.Property(slt => slt.TrackId)
            .IsRequired();

        // Relationships
        builder.HasOne<StudyLevel>()
            .WithMany()
            .HasForeignKey(slt => slt.StudyLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Track>()
            .WithMany()
            .HasForeignKey(slt => slt.TrackId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(slt => new { slt.StudyLevelId, slt.TrackId })
            .IsUnique()
            .HasDatabaseName("UQ_StudyLevelTrack_StudyLevel_Track");

        // Note: Seed data moved to DbInitializer.cs for runtime seeding
    }
}
