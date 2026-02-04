using LMS.Domain.Users;
using LMS.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for ParentProfile
/// </summary>
public sealed class ParentProfileConfiguration : IEntityTypeConfiguration<ParentProfile>
{
    public void Configure(EntityTypeBuilder<ParentProfile> builder)
    {
        builder.ToTable("ParentProfile");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.Id)
            .ValueGeneratedNever();

        // Configure UserId value object using centralized converter
        builder.Property(pp => pp.UserId)
            .HasConversion(StronglyTypedIdConverters.UserIdConverter)
            .IsRequired();

        builder.Property(pp => pp.RelationType)
            .HasMaxLength(50);

        builder.Property(pp => pp.IsFirstLogin)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pp => pp.CreatedAtUtc)
            .IsRequired();

        builder.Property(pp => pp.UpdatedAtUtc);

        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<ParentProfile>(pp => pp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pp => pp.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_ParentProfile_UserId");
    }
}
