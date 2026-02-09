using LMS.Domain.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplate");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Code).HasMaxLength(50).IsRequired();
        builder.Property(n => n.Name).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Type).HasConversion<int>().IsRequired();
        builder.Property(n => n.TitleTemplate).HasMaxLength(500).IsRequired();
        builder.Property(n => n.BodyTemplate).HasColumnType("text").IsRequired();
        builder.Property(n => n.TitleTemplateEn).HasMaxLength(500);
        builder.Property(n => n.BodyTemplateEn).HasColumnType("text");
        builder.Property(n => n.PushEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(n => n.EmailEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(n => n.SmsEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(n => n.InAppEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(n => n.Priority).HasConversion<int>().IsRequired();
        builder.Property(n => n.IconUrl).HasMaxLength(500);
        builder.Property(n => n.ActionUrl).HasMaxLength(500);
        builder.Property(n => n.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(n => n.CreatedAtUtc).IsRequired();
        builder.Property(n => n.UpdatedAtUtc).IsRequired();
        builder.HasIndex(n => n.Code).IsUnique().HasDatabaseName("UQ_NotificationTemplate_Code");
    }
}

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasConversion(id => id.Value, value => NotificationId.From(value)).ValueGeneratedNever();
        builder.Property(n => n.UserId).IsRequired();
        builder.Property(n => n.TemplateId);
        builder.Property(n => n.Type).HasConversion<int>().IsRequired();
        builder.Property(n => n.Title).HasMaxLength(500).IsRequired();
        builder.Property(n => n.Body).HasColumnType("text").IsRequired();
        builder.Property(n => n.ActionUrl).HasMaxLength(500);
        builder.Property(n => n.ImageUrl).HasMaxLength(500);
        builder.Property(n => n.Priority).HasConversion<int>().IsRequired();
        builder.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
        builder.Property(n => n.ReadAtUtc);
        builder.Property(n => n.ScheduledAtUtc);
        builder.Property(n => n.SentAtUtc);
        builder.Property(n => n.ExpiresAtUtc);
        builder.Property(n => n.CreatedAtUtc).IsRequired();
        builder.Property(n => n.UpdatedAtUtc).IsRequired();
        builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAtUtc }).HasDatabaseName("IX_Notification_User");
        builder.HasIndex(n => n.ScheduledAtUtc).HasDatabaseName("IX_Notification_Scheduled");
        builder.Ignore(n => n.DomainEvents);
    }
}

public sealed class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
{
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder.ToTable("NotificationDelivery");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.NotificationId).HasConversion(id => id.Value, value => NotificationId.From(value)).IsRequired();
        builder.Property(n => n.Channel).HasConversion<int>().IsRequired();
        builder.Property(n => n.Status).HasConversion<int>().IsRequired();
        builder.Property(n => n.ExternalId).HasMaxLength(200);
        builder.Property(n => n.ErrorMessage).HasColumnType("text");
        builder.Property(n => n.SentAtUtc);
        builder.Property(n => n.DeliveredAtUtc);
        builder.Property(n => n.CreatedAtUtc).IsRequired();
        builder.Property(n => n.UpdatedAtUtc).IsRequired();
        builder.HasIndex(n => new { n.NotificationId, n.Channel }).IsUnique().HasDatabaseName("UQ_NotificationDelivery");
        builder.HasIndex(n => new { n.Status, n.Channel }).HasDatabaseName("IX_NotificationDelivery_Status");
    }
}

public sealed class UserNotificationSettingsConfiguration : IEntityTypeConfiguration<UserNotificationSettings>
{
    public void Configure(EntityTypeBuilder<UserNotificationSettings> builder)
    {
        builder.ToTable("UserNotificationSettings");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.UserId).IsRequired();
        builder.Property(u => u.PushEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(u => u.EmailEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(u => u.SmsEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(u => u.DigestEmailEnabled).IsRequired().HasDefaultValue(true);
        builder.Property(u => u.DigestEmailTime).IsRequired();
        builder.Property(u => u.QuietHoursEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(u => u.QuietHoursStart);
        builder.Property(u => u.QuietHoursEnd);
        builder.Property(u => u.CreatedAtUtc).IsRequired();
        builder.Property(u => u.UpdatedAtUtc).IsRequired();
        builder.HasIndex(u => u.UserId).IsUnique().HasDatabaseName("UQ_UserNotificationSettings_User");
    }
}
