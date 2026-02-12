using LMS.Domain.Common;

namespace LMS.Domain.Purchasing;

public sealed class Product : AggregateRoot<ProductId>
{
    public ProductType ProductType { get; private set; }
    public Guid ReferenceId { get; private set; }
    public decimal? CashPrice { get; private set; }
    public int? PurchasingPointsPrice { get; private set; }
    public int PurchasingPointsReward { get; private set; }
    public decimal? DiscountPercentage { get; private set; }
    public bool IsActive { get; private set; }

    private Product() : base() { }

    private Product(
        ProductId id,
        ProductType productType,
        Guid referenceId,
        decimal? cashPrice,
        int? purchasingPointsPrice,
        int purchasingPointsReward,
        decimal? discountPercentage) : base(id)
    {
        ProductType = productType;
        ReferenceId = referenceId;
        CashPrice = cashPrice;
        PurchasingPointsPrice = purchasingPointsPrice;
        PurchasingPointsReward = purchasingPointsReward;
        DiscountPercentage = discountPercentage;
        IsActive = true;
    }

    public static Result<Product> Create(
        ProductType productType,
        Guid referenceId,
        decimal? cashPrice,
        int? purchasingPointsPrice,
        int purchasingPointsReward,
        decimal? discountPercentage = null)
    {
        if (cashPrice == null && purchasingPointsPrice == null)
            return Result<Product>.Failure(Error.Validation(ErrorCodes.Product.NoPriceSpecified));

        if (cashPrice.HasValue && cashPrice.Value < 0)
            return Result<Product>.Failure(Error.Validation(ErrorCodes.Product.InvalidCashPrice));

        if (purchasingPointsPrice.HasValue && purchasingPointsPrice.Value < 0)
            return Result<Product>.Failure(Error.Validation(ErrorCodes.Product.InvalidPointsPrice));

        if (purchasingPointsReward < 0)
            return Result<Product>.Failure(Error.Validation(ErrorCodes.Product.InvalidPointsReward));

        if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
            return Result<Product>.Failure(Error.Validation(ErrorCodes.Product.InvalidDiscountPercentage));

        if (referenceId == Guid.Empty)
            return Result<Product>.Failure(Error.Validation(ErrorCodes.Product.InvalidReferenceId));

        var product = new Product(
            ProductId.New(),
            productType,
            referenceId,
            cashPrice,
            purchasingPointsPrice,
            purchasingPointsReward,
            discountPercentage);

        return Result<Product>.Success(product);
    }

    public Result UpdatePricing(decimal? cashPrice, int? purchasingPointsPrice, int purchasingPointsReward)
    {
        if (cashPrice == null && purchasingPointsPrice == null)
            return Result.Failure(Error.Validation(ErrorCodes.Product.NoPriceSpecified));

        if (cashPrice.HasValue && cashPrice.Value < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Product.InvalidCashPrice));

        if (purchasingPointsPrice.HasValue && purchasingPointsPrice.Value < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Product.InvalidPointsPrice));

        if (purchasingPointsReward < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Product.InvalidPointsReward));

        CashPrice = cashPrice;
        PurchasingPointsPrice = purchasingPointsPrice;
        PurchasingPointsReward = purchasingPointsReward;

        return Result.Success();
    }

    public Result ApplyDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            return Result.Failure(Error.Validation(ErrorCodes.Product.InvalidDiscountPercentage));

        DiscountPercentage = discountPercentage;
        return Result.Success();
    }

    public void RemoveDiscount() => DiscountPercentage = null;

    public decimal? GetFinalCashPrice()
    {
        if (!CashPrice.HasValue) return null;
        if (!DiscountPercentage.HasValue) return CashPrice.Value;
        return CashPrice.Value * (1 - DiscountPercentage.Value / 100);
    }

    public int? GetFinalPointsPrice()
    {
        if (!PurchasingPointsPrice.HasValue) return null;
        if (!DiscountPercentage.HasValue) return PurchasingPointsPrice.Value;
        return (int)(PurchasingPointsPrice.Value * (1 - DiscountPercentage.Value / 100));
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
