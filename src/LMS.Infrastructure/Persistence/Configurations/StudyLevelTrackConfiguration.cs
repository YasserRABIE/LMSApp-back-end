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

        // Seed data - valid combinations
        // First Year: All tracks available
        builder.HasData(
            new
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                StudyLevelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                TrackId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                StudyLevelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                TrackId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                StudyLevelId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                TrackId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                IsActive = true
            },
            // Second Year: All tracks available
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                StudyLevelId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                TrackId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                StudyLevelId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                TrackId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                StudyLevelId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                TrackId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                IsActive = true
            },
            // Third Year (Tawjihi): All tracks available
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                StudyLevelId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                TrackId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                StudyLevelId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                TrackId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                StudyLevelId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                TrackId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                IsActive = true
            }
        );
    }
}
