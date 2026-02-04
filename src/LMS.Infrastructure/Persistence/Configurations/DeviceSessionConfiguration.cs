using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for DeviceSession
/// </summary>
public sealed class DeviceSessionConfiguration : IEntityTypeConfiguration<DeviceSession>
{
    public void Configure(EntityTypeBuilder<DeviceSession> builder)
    {
        builder.ToTable("DeviceSession");

        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.Id)
            .ValueGeneratedNever();

        // Configure UserId value object
        builder.Property(ds => ds.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .IsRequired();

        builder.Property(ds => ds.DeviceFingerprintHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ds => ds.Platform)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ds => ds.RefreshToken)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ds => ds.RefreshTokenExpiresAtUtc)
            .IsRequired();

        builder.Property(ds => ds.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ds => ds.LastIpAddress)
            .HasMaxLength(50);

        builder.Property(ds => ds.UserAgent)
            .HasMaxLength(500);

        builder.Property(ds => ds.CreatedAtUtc)
            .IsRequired();

        builder.Property(ds => ds.LastAccessedAtUtc)
            .IsRequired();

        builder.Property(ds => ds.RevokedAtUtc);

        // Relationships
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ds => ds.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ds => ds.UserId)
            .HasDatabaseName("IX_DeviceSession_UserId");

        builder.HasIndex(ds => ds.RefreshToken)
            .IsUnique()
            .HasDatabaseName("UQ_DeviceSession_RefreshToken");

        builder.HasIndex(ds => new { ds.UserId, ds.IsActive })
            .HasDatabaseName("IX_DeviceSession_UserId_IsActive");

        builder.HasIndex(ds => ds.RefreshTokenExpiresAtUtc)
            .HasDatabaseName("IX_DeviceSession_RefreshTokenExpiresAtUtc");
    }
}
