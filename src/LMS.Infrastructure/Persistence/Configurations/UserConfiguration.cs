using LMS.Domain.Common;
using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for User
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);

        // Configure UserId value object
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .ValueGeneratedNever();

        // Configure Phone value object
        builder.OwnsOne(u => u.Phone, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("Phone")
                .HasMaxLength(11)
                .IsRequired();

            phoneBuilder.HasIndex(p => p.Value)
                .IsUnique()
                .HasDatabaseName("UQ_User_Phone");
        });

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.SecondName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.UserType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.IsPhoneVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.Email)
            .HasMaxLength(255);

        builder.Property(u => u.CreatedAtUtc)
            .IsRequired();

        builder.Property(u => u.UpdatedAtUtc);

        // Indexes
        builder.HasIndex(u => u.UserType)
            .HasDatabaseName("IX_User_UserType");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_User_IsActive");

        // Ignore domain events (not persisted)
        builder.Ignore(u => u.DomainEvents);
    }
}
