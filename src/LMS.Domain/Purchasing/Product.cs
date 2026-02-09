using LMS.Domain.Common;

namespace LMS.Domain.Purchasing;

public sealed class Product : Entity<Guid>
{
    public ProductType ProductType { get; private set; }
    public Guid? ModuleId { get; private set; }
    public Guid? ContentItemId { get; private set; }
    public Guid? PointsPackageId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public decimal PriceEgp { get; private set; }
    public int PointsPrice { get; private set; }
    public bool AllowPointsPurchase { get; private set; }
    public decimal? DiscountedPriceEgp { get; private set; }
    public DateTime? DiscountStartUtc { get; private set; }
    public DateTime? DiscountEndUtc { get; private set; }
    public Guid? ParentProductId { get; private set; }
    public bool IsActive { get; private set; }
    public short DisplayOrder { get; private set; }

    private Product() : base() { Name = string.Empty; }

    public static Result<Product> Create(ProductType type, string name, decimal priceEgp, int pointsPrice, Guid? moduleId = null, Guid? contentItemId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result<Product>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        if (priceEgp < 0) return Result<Product>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        return Result<Product>.Success(new Product { Id = Guid.NewGuid(), ProductType = type, Name = name, PriceEgp = priceEgp, PointsPrice = pointsPrice, ModuleId = moduleId, ContentItemId = contentItemId, AllowPointsPurchase = true, IsActive = true });
    }

    public void SetDiscount(decimal discountedPrice, DateTime? startUtc, DateTime? endUtc) { DiscountedPriceEgp = discountedPrice; DiscountStartUtc = startUtc; DiscountEndUtc = endUtc; }
    public void ClearDiscount() { DiscountedPriceEgp = null; DiscountStartUtc = null; DiscountEndUtc = null; }
    public decimal GetEffectivePrice() => DiscountedPriceEgp ?? PriceEgp;
}

public sealed class PromoCode : Entity<Guid>
{
    public string Code { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public int? MaxTotalUses { get; private set; }
    public int CurrentTotalUses { get; private set; }
    public short MaxUsesPerUser { get; private set; }
    public decimal? MinOrderAmountEgp { get; private set; }
    public DateTime ValidFromUtc { get; private set; }
    public DateTime ValidUntilUtc { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private PromoCode() : base() { Code = string.Empty; }

    public static Result<PromoCode> Create(string code, DiscountType type, decimal value, DateTime validFrom, DateTime validUntil, Guid createdBy, short maxUsesPerUser = 1)
    {
        if (string.IsNullOrWhiteSpace(code)) return Result<PromoCode>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        if (value <= 0) return Result<PromoCode>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        return Result<PromoCode>.Success(new PromoCode { Id = Guid.NewGuid(), Code = code, DiscountType = type, DiscountValue = value, ValidFromUtc = validFrom, ValidUntilUtc = validUntil, CreatedByUserId = createdBy, MaxUsesPerUser = maxUsesPerUser, IsActive = true });
    }

    public Result<decimal> CalculateDiscount(decimal orderAmount)
    {
        if (!IsActive) return Result<decimal>.Failure(Error.Conflict("PROMO.INACTIVE"));
        if (DateTime.UtcNow < ValidFromUtc || DateTime.UtcNow > ValidUntilUtc) return Result<decimal>.Failure(Error.Conflict("PROMO.EXPIRED"));
        if (MinOrderAmountEgp.HasValue && orderAmount < MinOrderAmountEgp.Value) return Result<decimal>.Failure(Error.Conflict("PROMO.MIN_AMOUNT"));
        return Result<decimal>.Success(DiscountType == DiscountType.Percentage ? orderAmount * (DiscountValue / 100) : DiscountValue);
    }

    public void IncrementUsage() => CurrentTotalUses++;
}
