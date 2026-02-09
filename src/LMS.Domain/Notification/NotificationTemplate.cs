using LMS.Domain.Common;

namespace LMS.Domain.Notification;

public sealed class NotificationTemplate : Entity<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public NotificationType Type { get; private set; }
    public string TitleTemplate { get; private set; }
    public string BodyTemplate { get; private set; }
    public string? TitleTemplateEn { get; private set; }
    public string? BodyTemplateEn { get; private set; }
    public bool PushEnabled { get; private set; }
    public bool EmailEnabled { get; private set; }
    public bool SmsEnabled { get; private set; }
    public bool InAppEnabled { get; private set; }
    public Priority Priority { get; private set; }
    public string? IconUrl { get; private set; }
    public string? ActionUrl { get; private set; }
    public bool IsActive { get; private set; }

    private NotificationTemplate() : base() { Code = string.Empty; Name = string.Empty; TitleTemplate = string.Empty; BodyTemplate = string.Empty; }

    public static Result<NotificationTemplate> Create(string code, string name, NotificationType type, string titleTemplate, string bodyTemplate)
    {
        if (string.IsNullOrWhiteSpace(code)) return Result<NotificationTemplate>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<NotificationTemplate>.Success(new NotificationTemplate { Id = Guid.NewGuid(), Code = code, Name = name, Type = type, TitleTemplate = titleTemplate, BodyTemplate = bodyTemplate, PushEnabled = true, InAppEnabled = true, Priority = Priority.Normal, IsActive = true });
    }

    public void ConfigureChannels(bool push, bool email, bool sms, bool inApp) { PushEnabled = push; EmailEnabled = email; SmsEnabled = sms; InAppEnabled = inApp; }
}

public sealed class Notification : AggregateRoot<NotificationId>
{
    public Guid UserId { get; private set; }
    public Guid? TemplateId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public string? ActionUrl { get; private set; }
    public string? ImageUrl { get; private set; }
    public Priority Priority { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAtUtc { get; private set; }
    public DateTime? ScheduledAtUtc { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    private Notification() : base(NotificationId.New()) { Title = string.Empty; Body = string.Empty; }

    public static Result<Notification> Create(Guid userId, NotificationType type, string title, string body, Priority priority = Priority.Normal)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(title))
            return Result<Notification>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<Notification>.Success(new Notification { Id = NotificationId.New(), UserId = userId, Type = type, Title = title, Body = body, Priority = priority });
    }

    public void MarkAsRead() { IsRead = true; ReadAtUtc = DateTime.UtcNow; }
    public void Schedule(DateTime scheduledAtUtc) => ScheduledAtUtc = scheduledAtUtc;
    public void MarkAsSent() => SentAtUtc = DateTime.UtcNow;
}

public sealed class NotificationDelivery : Entity<long>
{
    public NotificationId NotificationId { get; private set; }
    public DeliveryChannel Channel { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public string? ExternalId { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }

    private NotificationDelivery() : base() { NotificationId = NotificationId.New(); }

    public static Result<NotificationDelivery> Create(NotificationId notificationId, DeliveryChannel channel)
    {
        if (notificationId.Value == Guid.Empty) return Result<NotificationDelivery>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<NotificationDelivery>.Success(new NotificationDelivery { NotificationId = notificationId, Channel = channel, Status = DeliveryStatus.Pending });
    }

    public void MarkAsSent(string? externalId = null) { Status = DeliveryStatus.Sent; SentAtUtc = DateTime.UtcNow; ExternalId = externalId; }
    public void MarkAsDelivered() { Status = DeliveryStatus.Delivered; DeliveredAtUtc = DateTime.UtcNow; }
    public void MarkAsFailed(string errorMessage) { Status = DeliveryStatus.Failed; ErrorMessage = errorMessage; }
}

public sealed class UserNotificationSettings : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public bool PushEnabled { get; private set; }
    public bool EmailEnabled { get; private set; }
    public bool SmsEnabled { get; private set; }
    public bool DigestEmailEnabled { get; private set; }
    public TimeSpan DigestEmailTime { get; private set; }
    public bool QuietHoursEnabled { get; private set; }
    public TimeSpan? QuietHoursStart { get; private set; }
    public TimeSpan? QuietHoursEnd { get; private set; }

    private UserNotificationSettings() : base() { }

    public static Result<UserNotificationSettings> Create(Guid userId)
    {
        if (userId == Guid.Empty) return Result<UserNotificationSettings>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<UserNotificationSettings>.Success(new UserNotificationSettings { Id = Guid.NewGuid(), UserId = userId, PushEnabled = true, EmailEnabled = true, DigestEmailEnabled = true, DigestEmailTime = new TimeSpan(8, 0, 0) });
    }

    public void UpdateChannels(bool push, bool email, bool sms) { PushEnabled = push; EmailEnabled = email; SmsEnabled = sms; }
    public void ConfigureQuietHours(bool enabled, TimeSpan? start = null, TimeSpan? end = null) { QuietHoursEnabled = enabled; QuietHoursStart = start; QuietHoursEnd = end; }
}
