using LMS.Domain.Users;
using LMS.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for TeacherProfile
/// </summary>
public sealed class TeacherProfileConfiguration : IEntityTypeConfiguration<TeacherProfile>
{
    public void Configure(EntityTypeBuilder<TeacherProfile> builder)
    {
        builder.ToTable("TeacherProfile");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Id)
            .ValueGeneratedNever();

        // Configure UserId value object using centralized converter
        builder.Property(tp => tp.UserId)
            .HasConversion(StronglyTypedIdConverters.UserIdConverter)
            .IsRequired();

        builder.Property(tp => tp.Bio)
            .HasColumnType("text");

        builder.Property(tp => tp.Specialization)
            .HasMaxLength(200);

        builder.Property(tp => tp.YearsOfExperience);

        builder.Property(tp => tp.Qualifications)
            .HasColumnType("text");

        builder.Property(tp => tp.SocialLinks)
            .HasColumnType("text");

        builder.Property(tp => tp.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(tp => tp.CreatedAtUtc)
            .IsRequired();

        builder.Property(tp => tp.UpdatedAtUtc);

        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<TeacherProfile>(tp => tp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(tp => tp.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_TeacherProfile_UserId");
    }
}
