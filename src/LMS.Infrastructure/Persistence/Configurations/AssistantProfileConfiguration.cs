using LMS.Domain.Users;
using LMS.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for AssistantProfile
/// </summary>
public sealed class AssistantProfileConfiguration : IEntityTypeConfiguration<AssistantProfile>
{
    public void Configure(EntityTypeBuilder<AssistantProfile> builder)
    {
        builder.ToTable("AssistantProfile");

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Id)
            .ValueGeneratedNever();

        // Configure UserId value object using centralized converter
        builder.Property(ap => ap.UserId)
            .HasConversion(StronglyTypedIdConverters.UserIdConverter)
            .IsRequired();

        builder.Property(ap => ap.Bio)
            .HasColumnType("text");

        builder.Property(ap => ap.MaxStudents)
            .IsRequired()
            .HasDefaultValue(50);

        builder.Property(ap => ap.CurrentStudentCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ap => ap.AverageRating)
            .HasColumnType("decimal(3,2)")
            .HasDefaultValue(0);

        builder.Property(ap => ap.TotalRatings)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ap => ap.CreatedAtUtc)
            .IsRequired();

        builder.Property(ap => ap.UpdatedAtUtc);

        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<AssistantProfile>(ap => ap.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ap => ap.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_AssistantProfile_UserId");

        builder.HasIndex(ap => ap.AverageRating)
            .HasDatabaseName("IX_AssistantProfile_AverageRating");
    }
}
