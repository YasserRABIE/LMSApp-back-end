using LMS.Domain.Common;
using LMS.Domain.Users;
using LMS.Domain.Purchasing;

namespace LMS.Domain.Content;

public sealed class Course : AggregateRoot<CourseId>
{
    public UserId TeacherId { get; private set; }
    public SubjectId SubjectId { get; private set; }
    public Guid StudyLevelId { get; private set; }
    public Guid TrackId { get; private set; }
    public SchoolTypeId SchoolTypeId { get; private set; }
    public CourseCategoryId CourseCategoryId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Thumbnail { get; private set; }
    public Visibility Visibility { get; private set; }
    public ProductId? ProductId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }

    private Course() : base()
    {
        TeacherId = null!;
        SubjectId = null!;
        SchoolTypeId = null!;
        CourseCategoryId = null!;
        Title = string.Empty;
        Description = string.Empty;
        Thumbnail = string.Empty;
    }

    private Course(
        CourseId id,
        UserId teacherId,
        SubjectId subjectId,
        Guid studyLevelId,
        Guid trackId,
        SchoolTypeId schoolTypeId,
        CourseCategoryId courseCategoryId,
        string title,
        string description,
        string thumbnail) : base(id)
    {
        TeacherId = teacherId;
        SubjectId = subjectId;
        StudyLevelId = studyLevelId;
        TrackId = trackId;
        SchoolTypeId = schoolTypeId;
        CourseCategoryId = courseCategoryId;
        Title = title;
        Description = description;
        Thumbnail = thumbnail;
        Visibility = Visibility.Hidden;
        IsActive = true;
    }

    public static Result<Course> Create(
        UserId teacherId,
        SubjectId subjectId,
        Guid studyLevelId,
        Guid trackId,
        SchoolTypeId schoolTypeId,
        CourseCategoryId courseCategoryId,
        string title,
        string description,
        string thumbnail)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Course>.Failure(Error.Validation(ErrorCodes.Course.TitleRequired));

        if (title.Length > 300)
            return Result<Course>.Failure(Error.Validation(ErrorCodes.Course.TitleTooLong));

        if (string.IsNullOrWhiteSpace(description))
            return Result<Course>.Failure(Error.Validation(ErrorCodes.Course.DescriptionRequired));

        if (string.IsNullOrWhiteSpace(thumbnail))
            return Result<Course>.Failure(Error.Validation(ErrorCodes.Course.ThumbnailRequired));

        if (studyLevelId == Guid.Empty)
            return Result<Course>.Failure(Error.Validation(ErrorCodes.Course.InvalidStudyLevel));

        if (trackId == Guid.Empty)
            return Result<Course>.Failure(Error.Validation(ErrorCodes.Course.InvalidTrack));

        var course = new Course(
            CourseId.New(),
            teacherId,
            subjectId,
            studyLevelId,
            trackId,
            schoolTypeId,
            courseCategoryId,
            title.Trim(),
            description.Trim(),
            thumbnail.Trim());

        return Result<Course>.Success(course);
    }

    public Result UpdateDetails(
        SubjectId subjectId,
        string title,
        string description,
        string thumbnail,
        SchoolTypeId schoolTypeId,
        CourseCategoryId courseCategoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation(ErrorCodes.Course.TitleRequired));

        if (title.Length > 300)
            return Result.Failure(Error.Validation(ErrorCodes.Course.TitleTooLong));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(Error.Validation(ErrorCodes.Course.DescriptionRequired));

        if (string.IsNullOrWhiteSpace(thumbnail))
            return Result.Failure(Error.Validation(ErrorCodes.Course.ThumbnailRequired));

        SubjectId = subjectId;
        Title = title.Trim();
        Description = description.Trim();
        Thumbnail = thumbnail.Trim();
        SchoolTypeId = schoolTypeId;
        CourseCategoryId = courseCategoryId;

        return Result.Success();
    }

    public Result Publish()
    {
        if (Visibility == Visibility.Published)
            return Result.Failure(Error.Conflict(ErrorCodes.Course.AlreadyPublished));

        Visibility = Visibility.Published;
        PublishedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public void ChangeVisibility(Visibility visibility)
    {
        Visibility = visibility;
        if (visibility == Visibility.Published && PublishedAtUtc == null)
        {
            PublishedAtUtc = DateTime.UtcNow;
        }
    }

    public void Hide() => Visibility = Visibility.Hidden;
    public void Archive() => Visibility = Visibility.Archived;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void AssignProduct(ProductId productId) => ProductId = productId;
    public void RemoveProduct() => ProductId = null;
}
