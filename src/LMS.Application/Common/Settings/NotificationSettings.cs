namespace LMS.Application.Common.Settings;

public sealed class NotificationSettings
{
    public bool EnablePush { get; init; }
    public bool EnableEmail { get; init; }
    public bool EnableSms { get; init; }
    public string PushProvider { get; init; } = string.Empty;
    public string EmailProvider { get; init; } = string.Empty;
    public string SmsProvider { get; init; } = string.Empty;
    public int DigestEmailHourUtc { get; init; }
    public int MaxNotificationsPerDay { get; init; }
}
