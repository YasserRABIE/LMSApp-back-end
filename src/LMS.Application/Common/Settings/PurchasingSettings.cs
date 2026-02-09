namespace LMS.Application.Common.Settings;

public sealed class PurchasingSettings
{
    public int PointsPerEgp { get; init; }
    public int MinPointsForPurchase { get; init; }
    public int MaxPointsPercentagePerOrder { get; init; }
    public int PendingOrderExpiryMinutes { get; init; }
    public bool EnablePromoCode { get; init; }
    public bool EnableRefunds { get; init; }
    public int RefundWindowDays { get; init; }
}
