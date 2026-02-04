using LMS.Domain.Users;
using LMS.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for StudentProfile
/// </summary>
public sealed class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable("StudentProfile");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
            .ValueGeneratedNever();

        // Configure UserId value object using centralized converter
        builder.Property(sp => sp.UserId)
            .HasConversion(StronglyTypedIdConverters.UserIdConverter)
            .IsRequired();

        builder.Property(sp => sp.StudyLevelTrackId)
            .IsRequired();

        builder.Property(sp => sp.SchoolName)
            .HasMaxLength(200);

        builder.Property(sp => sp.Governorate)
            .HasMaxLength(100);

        builder.Property(sp => sp.CreatedAtUtc)
            .IsRequired();

        builder.Property(sp => sp.UpdatedAtUtc);

        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<StudentProfile>(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<StudyLevelTrack>()
            .WithMany()
            .HasForeignKey(sp => sp.StudyLevelTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(sp => sp.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_StudentProfile_UserId");

        builder.HasIndex(sp => sp.StudyLevelTrackId)
            .HasDatabaseName("IX_StudentProfile_StudyLevelTrackId");
    }
}
