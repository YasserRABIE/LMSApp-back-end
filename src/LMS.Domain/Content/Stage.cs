using LMS.Domain.Common;
using LMS.Domain.Purchasing;

namespace LMS.Domain.Content;

public sealed class Stage : Entity<StageId>
{
    public ModuleId ModuleId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public Visibility Visibility { get; private set; }
    public ProductId? ProductId { get; private set; }
    public bool IsActive { get; private set; }

    private Stage() : base()
    {
        ModuleId = null!;
        Title = string.Empty;
    }

    private Stage(StageId id, ModuleId moduleId, string title, string? description, int displayOrder) : base(id)
    {
        ModuleId = moduleId;
        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        Visibility = Visibility.Hidden;
        IsActive = true;
    }

    public static Result<Stage> Create(ModuleId moduleId, string title, string? description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Stage>.Failure(Error.Validation(ErrorCodes.Stage.TitleRequired));

        if (title.Length > 300)
            return Result<Stage>.Failure(Error.Validation(ErrorCodes.Stage.TitleTooLong));

        if (displayOrder < 0)
            return Result<Stage>.Failure(Error.Validation(ErrorCodes.Stage.InvalidDisplayOrder));

        var stage = new Stage(StageId.New(), moduleId, title.Trim(), description?.Trim(), displayOrder);
        return Result<Stage>.Success(stage);
    }

    public Result UpdateDetails(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation(ErrorCodes.Stage.TitleRequired));

        if (title.Length > 300)
            return Result.Failure(Error.Validation(ErrorCodes.Stage.TitleTooLong));

        Title = title.Trim();
        Description = description?.Trim();

        return Result.Success();
    }

    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
    public void Publish() => Visibility = Visibility.Published;
    public void Hide() => Visibility = Visibility.Hidden;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void AssignProduct(ProductId productId) => ProductId = productId;
    public void RemoveProduct() => ProductId = null;
}
