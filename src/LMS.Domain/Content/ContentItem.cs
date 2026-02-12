using LMS.Domain.Common;
using LMS.Domain.Purchasing;

namespace LMS.Domain.Content;

public sealed class ContentItem : Entity<ContentItemId>
{
    public StageId StageId { get; private set; }
    public ContentType ContentType { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsFreePreview { get; private set; }
    public int XpReward { get; private set; }
    public int PurchasingPointsReward { get; private set; }
    public ProductId? ProductId { get; private set; }
    public bool IsActive { get; private set; }

    private ContentItem() : base()
    {
        StageId = null!;
        Title = string.Empty;
    }

    private ContentItem(
        ContentItemId id,
        StageId stageId,
        ContentType contentType,
        string title,
        string? description,
        int displayOrder,
        bool isFreePreview,
        int xpReward,
        int purchasingPointsReward) : base(id)
    {
        StageId = stageId;
        ContentType = contentType;
        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        IsFreePreview = isFreePreview;
        XpReward = xpReward;
        PurchasingPointsReward = purchasingPointsReward;
        IsActive = true;
    }

    public static Result<ContentItem> Create(
        StageId stageId,
        ContentType contentType,
        string title,
        string? description,
        int displayOrder,
        int xpReward = 0,
        int purchasingPointsReward = 0)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<ContentItem>.Failure(Error.Validation(ErrorCodes.Content.TitleRequired));

        if (title.Length > 300)
            return Result<ContentItem>.Failure(Error.Validation(ErrorCodes.Content.TitleTooLong));

        if (displayOrder < 0)
            return Result<ContentItem>.Failure(Error.Validation(ErrorCodes.Content.InvalidDisplayOrder));

        if (xpReward < 0)
            return Result<ContentItem>.Failure(Error.Validation(ErrorCodes.Content.InvalidXpReward));

        if (purchasingPointsReward < 0)
            return Result<ContentItem>.Failure(Error.Validation(ErrorCodes.Content.InvalidPurchasingPointsReward));

        var contentItem = new ContentItem(
            ContentItemId.New(),
            stageId,
            contentType,
            title.Trim(),
            description?.Trim(),
            displayOrder,
            false,
            xpReward,
            purchasingPointsReward);

        return Result<ContentItem>.Success(contentItem);
    }

    public Result UpdateDetails(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation(ErrorCodes.Content.TitleRequired));

        if (title.Length > 300)
            return Result.Failure(Error.Validation(ErrorCodes.Content.TitleTooLong));

        Title = title.Trim();
        Description = description?.Trim();

        return Result.Success();
    }

    public Result SetRewards(int xpReward, int purchasingPointsReward)
    {
        if (xpReward < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Content.InvalidXpReward));

        if (purchasingPointsReward < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Content.InvalidPurchasingPointsReward));

        XpReward = xpReward;
        PurchasingPointsReward = purchasingPointsReward;

        return Result.Success();
    }

    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
    public void EnableFreePreview() => IsFreePreview = true;
    public void DisableFreePreview() => IsFreePreview = false;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void AssignProduct(ProductId productId) => ProductId = productId;
    public void RemoveProduct() => ProductId = null;
}
