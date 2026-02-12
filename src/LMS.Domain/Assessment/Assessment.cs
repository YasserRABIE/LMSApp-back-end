using LMS.Domain.Common;
using LMS.Domain.Content;

namespace LMS.Domain.Assessment;

/// <summary>
/// Assessment aggregate root
/// Represents a quiz, assignment, or final exam
/// </summary>
public sealed class Assessment : AggregateRoot<AssessmentId>
{
    /// <summary>
    /// Reference to associated content item (if this assessment is content)
    /// </summary>
    public Guid? ContentItemId { get; private set; }

    /// <summary>
    /// Reference to the course
    /// </summary>
    public CourseId CourseId { get; private set; }

    /// <summary>
    /// Reference to the module (if module-specific)
    /// </summary>
    public Guid? ModuleId { get; private set; }

    /// <summary>
    /// Type of assessment
    /// </summary>
    public AssessmentType AssessmentType { get; private set; }

    /// <summary>
    /// Assessment title
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Instructions for students
    /// </summary>
    public string? Instructions { get; private set; }

    /// <summary>
    /// Total marks available
    /// </summary>
    public decimal TotalMarks { get; private set; }

    /// <summary>
    /// Marks required to pass
    /// </summary>
    public decimal PassingMarks { get; private set; }

    /// <summary>
    /// Time limit in minutes (null for no limit)
    /// </summary>
    public short? DurationMinutes { get; private set; }

    /// <summary>
    /// Maximum number of attempts allowed
    /// </summary>
    public short MaxRetakes { get; private set; }

    /// <summary>
    /// Minimum minutes between retakes
    /// </summary>
    public short RetakeCooldownMinutes { get; private set; }

    /// <summary>
    /// Whether to randomize question order
    /// </summary>
    public bool ShuffleQuestions { get; private set; }

    /// <summary>
    /// Whether to randomize option order (for MCQs)
    /// </summary>
    public bool ShuffleOptions { get; private set; }

    /// <summary>
    /// Whether to show correct answers after submission
    /// </summary>
    public bool ShowCorrectAnswers { get; private set; }

    /// <summary>
    /// Whether to allow late submissions
    /// </summary>
    public bool AllowLateSubmission { get; private set; }

    /// <summary>
    /// When the assessment becomes available
    /// </summary>
    public DateTime? AvailableFromUtc { get; private set; }

    /// <summary>
    /// Assessment deadline
    /// </summary>
    public DateTime? DeadlineUtc { get; private set; }

    /// <summary>
    /// XP points awarded for passing (on-time)
    /// </summary>
    public short XpReward { get; private set; }

    /// <summary>
    /// Game points awarded for passing (on-time)
    /// </summary>
    public short PointsReward { get; private set; }

    /// <summary>
    /// XP points awarded for passing (late submission)
    /// </summary>
    public short XpRewardIfLate { get; private set; }

    /// <summary>
    /// Game points awarded for passing (late submission)
    /// </summary>
    public short PointsRewardIfLate { get; private set; }

    /// <summary>
    /// Reference to grading rubric (for manual grading)
    /// </summary>
    public Guid? GradingRubricId { get; private set; }

    /// <summary>
    /// Assessment visibility status
    /// </summary>
    public Visibility Visibility { get; private set; }

    /// <summary>
    /// Indicates if this assessment is active
    /// </summary>
    public bool IsActive { get; private set; }

    // EF Core constructor
    private Assessment() : base(AssessmentId.New())
    {
        Title = string.Empty;
        CourseId = CourseId.New();
    }

    private Assessment(
        AssessmentId id,
        CourseId courseId,
        AssessmentType assessmentType,
        string title,
        decimal totalMarks,
        decimal passingMarks,
        short maxRetakes = 5,
        short retakeCooldownMinutes = 5,
        Guid? contentItemId = null,
        Guid? moduleId = null,
        string? instructions = null,
        short? durationMinutes = null,
        DateTime? availableFromUtc = null,
        DateTime? deadlineUtc = null,
        Guid? gradingRubricId = null)
        : base(id)
    {
        ContentItemId = contentItemId;
        CourseId = courseId;
        ModuleId = moduleId;
        AssessmentType = assessmentType;
        Title = title;
        Instructions = instructions;
        TotalMarks = totalMarks;
        PassingMarks = passingMarks;
        DurationMinutes = durationMinutes;
        MaxRetakes = maxRetakes;
        RetakeCooldownMinutes = retakeCooldownMinutes;
        ShuffleQuestions = true;
        ShuffleOptions = true;
        ShowCorrectAnswers = true;
        AllowLateSubmission = true;
        AvailableFromUtc = availableFromUtc;
        DeadlineUtc = deadlineUtc;
        XpReward = 0;
        PointsReward = 0;
        XpRewardIfLate = 0;
        PointsRewardIfLate = 0;
        GradingRubricId = gradingRubricId;
        Visibility = Visibility.Hidden;
        IsActive = true;
    }

    /// <summary>
    /// Factory method to create a new assessment
    /// </summary>
    public static Result<Assessment> Create(
        CourseId courseId,
        AssessmentType assessmentType,
        string title,
        decimal totalMarks,
        decimal passingMarks,
        short maxRetakes = 5,
        short retakeCooldownMinutes = 5,
        Guid? contentItemId = null,
        Guid? moduleId = null,
        string? instructions = null,
        short? durationMinutes = null,
        DateTime? availableFromUtc = null,
        DateTime? deadlineUtc = null,
        Guid? gradingRubricId = null)
    {
        if (courseId.Value == Guid.Empty)
        {
            return Result<Assessment>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Assessment>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (title.Length > 300)
        {
            return Result<Assessment>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (passingMarks <= 0 || passingMarks > totalMarks)
        {
            return Result<Assessment>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (maxRetakes < 1)
        {
            return Result<Assessment>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (availableFromUtc.HasValue && deadlineUtc.HasValue && deadlineUtc <= availableFromUtc)
        {
            return Result<Assessment>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var assessment = new Assessment(
            AssessmentId.New(),
            courseId,
            assessmentType,
            title,
            totalMarks,
            passingMarks,
            maxRetakes,
            retakeCooldownMinutes,
            contentItemId,
            moduleId,
            instructions,
            durationMinutes,
            availableFromUtc,
            deadlineUtc,
            gradingRubricId
        );

        return Result<Assessment>.Success(assessment);
    }

    /// <summary>
    /// Updates assessment details
    /// </summary>
    public Result UpdateDetails(
        string title,
        decimal totalMarks,
        decimal passingMarks,
        string? instructions = null,
        short? durationMinutes = null)
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

        if (passingMarks <= 0 || passingMarks > totalMarks)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        Title = title;
        TotalMarks = totalMarks;
        PassingMarks = passingMarks;
        Instructions = instructions;
        DurationMinutes = durationMinutes;

        return Result.Success();
    }

    /// <summary>
    /// Sets the assessment availability window
    /// </summary>
    public Result SetAvailability(DateTime? availableFromUtc, DateTime? deadlineUtc)
    {
        if (availableFromUtc.HasValue && deadlineUtc.HasValue && deadlineUtc <= availableFromUtc)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        AvailableFromUtc = availableFromUtc;
        DeadlineUtc = deadlineUtc;

        return Result.Success();
    }

    /// <summary>
    /// Configures retake settings
    /// </summary>
    public Result ConfigureRetakes(short maxRetakes, short cooldownMinutes)
    {
        if (maxRetakes < 1)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        MaxRetakes = maxRetakes;
        RetakeCooldownMinutes = cooldownMinutes;

        return Result.Success();
    }

    /// <summary>
    /// Configures assessment behavior settings
    /// </summary>
    public void ConfigureBehavior(
        bool shuffleQuestions,
        bool shuffleOptions,
        bool showCorrectAnswers,
        bool allowLateSubmission)
    {
        ShuffleQuestions = shuffleQuestions;
        ShuffleOptions = shuffleOptions;
        ShowCorrectAnswers = showCorrectAnswers;
        AllowLateSubmission = allowLateSubmission;
    }

    /// <summary>
    /// Sets the gamification rewards
    /// </summary>
    public Result SetRewards(
        short xpReward,
        short pointsReward,
        short xpRewardIfLate,
        short pointsRewardIfLate)
    {
        if (xpReward < 0 || pointsReward < 0 || xpRewardIfLate < 0 || pointsRewardIfLate < 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        XpReward = xpReward;
        PointsReward = pointsReward;
        XpRewardIfLate = xpRewardIfLate;
        PointsRewardIfLate = pointsRewardIfLate;

        return Result.Success();
    }

    /// <summary>
    /// Associates this assessment with a grading rubric
    /// </summary>
    public void SetGradingRubric(Guid? gradingRubricId) => GradingRubricId = gradingRubricId;

    /// <summary>
    /// Publishes the assessment (makes it visible to students)
    /// </summary>
    public Result Publish()
    {
        if (Visibility == Visibility.Published)
        {
            return Result.Failure(
                Error.Conflict("ASSESSMENT.ALREADY_PUBLISHED")
            );
        }

        Visibility = Visibility.Published;
        return Result.Success();
    }

    /// <summary>
    /// Unpublishes the assessment
    /// </summary>
    public void Unpublish() => Visibility = Visibility.Hidden;

    /// <summary>
    /// Hides the assessment completely
    /// </summary>
    public void Hide() => Visibility = Visibility.Hidden;

    /// <summary>
    /// Deactivates the assessment
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the assessment
    /// </summary>
    public void Activate() => IsActive = true;
}
