using LMS.Domain.Common;
using LMS.Domain.Purchasing;

namespace LMS.Domain.Content;

public sealed class Module : Entity<ModuleId>
{
    public CourseId CourseId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public string? Thumbnail { get; private set; }
    public decimal EstimatedHours { get; private set; }
    public ProductId? ProductId { get; private set; }
    public bool IsActive { get; private set; }

    private Module() : base()
    {
        CourseId = null!;
        Title = string.Empty;
    }

    private Module(ModuleId id, CourseId courseId, string title, string? description, int displayOrder, decimal estimatedHours, string? thumbnail) : base(id)
    {
        CourseId = courseId;
        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        EstimatedHours = estimatedHours;
        Thumbnail = thumbnail;
        IsActive = true;
    }

    public static Result<Module> Create(CourseId courseId, string title, string? description, int displayOrder, decimal estimatedHours, string? thumbnail = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Module>.Failure(Error.Validation(ErrorCodes.Module.TitleRequired));

        if (title.Length > 300)
            return Result<Module>.Failure(Error.Validation(ErrorCodes.Module.TitleTooLong));

        if (displayOrder < 0)
            return Result<Module>.Failure(Error.Validation(ErrorCodes.Module.InvalidDisplayOrder));

        if (estimatedHours < 0)
            return Result<Module>.Failure(Error.Validation(ErrorCodes.Module.InvalidEstimatedHours));

        var module = new Module(ModuleId.New(), courseId, title.Trim(), description?.Trim(), displayOrder, estimatedHours, thumbnail?.Trim());
        return Result<Module>.Success(module);
    }

    public Result UpdateDetails(string title, string? description, decimal estimatedHours, string? thumbnail)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation(ErrorCodes.Module.TitleRequired));

        if (title.Length > 300)
            return Result.Failure(Error.Validation(ErrorCodes.Module.TitleTooLong));

        if (estimatedHours < 0)
            return Result.Failure(Error.Validation(ErrorCodes.Module.InvalidEstimatedHours));

        Title = title.Trim();
        Description = description?.Trim();
        EstimatedHours = estimatedHours;
        Thumbnail = thumbnail?.Trim();

        return Result.Success();
    }

    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void AssignProduct(ProductId productId) => ProductId = productId;
    public void RemoveProduct() => ProductId = null;
}
