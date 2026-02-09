using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Course aggregate root
/// Represents a complete course for a specific subject and study level/track combination
/// </summary>
public sealed class Course : AggregateRoot<CourseId>
{
    /// <summary>
    /// Reference to the study level and track combination
    /// </summary>
    public Guid StudyLevelTrackId { get; private set; }

    /// <summary>
    /// Reference to the subject
    /// </summary>
    public Guid SubjectId { get; private set; }

    /// <summary>
    /// Reference to the teacher who created this course
    /// </summary>
    public Guid TeacherId { get; private set; }

    /// <summary>
    /// Course title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Course description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Thumbnail image URL
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Intro video URL (for preview/marketing)
    /// </summary>
    public string? IntroVideoUrl { get; private set; }

    /// <summary>
    /// Full course price (if purchasing entire course)
    /// </summary>
    public decimal FullPrice { get; private set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public short DisplayOrder { get; private set; }

    /// <summary>
    /// Course visibility status
    /// </summary>
    public Visibility Visibility { get; private set; }

    /// <summary>
    /// Indicates if this course is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Current version number (for content updates)
    /// </summary>
    public int CurrentVersion { get; private set; }

    // EF Core constructor
    private Course() : base(CourseId.New())
    {
        Title = string.Empty;
    }

    private Course(
        CourseId id,
        Guid studyLevelTrackId,
        Guid subjectId,
        Guid teacherId,
        string title,
        string? description,
        decimal fullPrice,
        short displayOrder)
        : base(id)
    {
        StudyLevelTrackId = studyLevelTrackId;
        SubjectId = subjectId;
        TeacherId = teacherId;
        Title = title;
        Description = description;
        FullPrice = fullPrice;
        DisplayOrder = displayOrder;
        Visibility = Visibility.Draft;
        IsActive = true;
        CurrentVersion = 1;
    }

    /// <summary>
    /// Factory method to create a new course
    /// </summary>
    public static Result<Course> Create(
        Guid studyLevelTrackId,
        Guid subjectId,
        Guid teacherId,
        string title,
        string? description,
        decimal fullPrice,
        short displayOrder)
    {
        // Validate required fields
        if (studyLevelTrackId == Guid.Empty)
        {
            return Result<Course>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (subjectId == Guid.Empty)
        {
            return Result<Course>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (teacherId == Guid.Empty)
        {
            return Result<Course>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Course>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result<Course>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (fullPrice < 0)
        {
            return Result<Course>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var course = new Course(
            CourseId.New(),
            studyLevelTrackId,
            subjectId,
            teacherId,
            title,
            description,
            fullPrice,
            displayOrder
        );

        return Result<Course>.Success(course);
    }

    /// <summary>
    /// Updates course details
    /// </summary>
    public Result UpdateDetails(
        string title,
        string? description,
        decimal fullPrice,
        short displayOrder)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (fullPrice < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Title = title;
        Description = description;
        FullPrice = fullPrice;
        DisplayOrder = displayOrder;

        return Result.Success();
    }

    /// <summary>
    /// Sets the course thumbnail
    /// </summary>
    public void SetThumbnail(string? thumbnailUrl) => ThumbnailUrl = thumbnailUrl;

    /// <summary>
    /// Sets the intro video URL
    /// </summary>
    public void SetIntroVideo(string? introVideoUrl) => IntroVideoUrl = introVideoUrl;

    /// <summary>
    /// Publishes the course (makes it visible to students)
    /// </summary>
    public Result Publish()
    {
        if (Visibility == Visibility.Published)
        {
            return Result.Failure(
                Error.Conflict("COURSE.ALREADY_PUBLISHED")
            );
        }

        Visibility = Visibility.Published;
        return Result.Success();
    }

    /// <summary>
    /// Unpublishes the course (hides from students)
    /// </summary>
    public Result Unpublish()
    {
        if (Visibility != Visibility.Published)
        {
            return Result.Failure(
                Error.Conflict("COURSE.NOT_PUBLISHED")
            );
        }

        Visibility = Visibility.Draft;
        return Result.Success();
    }

    /// <summary>
    /// Hides the course completely
    /// </summary>
    public void Hide() => Visibility = Visibility.Hidden;

    /// <summary>
    /// Sets the course to draft state
    /// </summary>
    public void SetDraft() => Visibility = Visibility.Draft;

    /// <summary>
    /// Increments the course version (when content is updated significantly)
    /// </summary>
    public void IncrementVersion() => CurrentVersion++;

    /// <summary>
    /// Deactivates the course
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the course
    /// </summary>
    public void Activate() => IsActive = true;
}
