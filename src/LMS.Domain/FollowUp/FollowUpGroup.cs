using LMS.Domain.Common;
using LMS.Domain.Content;

namespace LMS.Domain.FollowUp;

/// <summary>
/// FollowUpGroup aggregate root
/// Represents a follow-up group managed by an assistant for a specific module
/// </summary>
public sealed class FollowUpGroup : AggregateRoot<FollowUpGroupId>
{
    public string Name { get; private set; }
    public CourseId CourseId { get; private set; }
    public Guid ModuleId { get; private set; }
    public Guid StudyLevelTrackId { get; private set; }
    public Guid AssistantId { get; private set; }
    public short MaxStudents { get; private set; }
    public short CurrentStudentCount { get; private set; }
    public bool IsAcceptingNewStudents { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    private FollowUpGroup() : base(FollowUpGroupId.New())
    {
        Name = string.Empty;
        CourseId = CourseId.New();
    }

    private FollowUpGroup(
        FollowUpGroupId id,
        string name,
        CourseId courseId,
        Guid moduleId,
        Guid studyLevelTrackId,
        Guid assistantId,
        short maxStudents,
        Guid createdByUserId)
        : base(id)
    {
        Name = name;
        CourseId = courseId;
        ModuleId = moduleId;
        StudyLevelTrackId = studyLevelTrackId;
        AssistantId = assistantId;
        MaxStudents = maxStudents;
        CurrentStudentCount = 0;
        IsAcceptingNewStudents = true;
        IsActive = true;
        CreatedByUserId = createdByUserId;
    }

    public static Result<FollowUpGroup> Create(
        string name,
        CourseId courseId,
        Guid moduleId,
        Guid studyLevelTrackId,
        Guid assistantId,
        short maxStudents,
        Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<FollowUpGroup>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        if (name.Length > 200)
            return Result<FollowUpGroup>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        if (maxStudents <= 0)
            return Result<FollowUpGroup>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));

        return Result<FollowUpGroup>.Success(new FollowUpGroup(FollowUpGroupId.New(), name, courseId, moduleId, studyLevelTrackId, assistantId, maxStudents, createdByUserId));
    }

    public Result AddStudent()
    {
        if (CurrentStudentCount >= MaxStudents)
            return Result.Failure(Error.Conflict("GROUP.FULL"));
        CurrentStudentCount++;
        if (CurrentStudentCount >= MaxStudents)
            IsAcceptingNewStudents = false;
        return Result.Success();
    }

    public void RemoveStudent()
    {
        if (CurrentStudentCount > 0)
            CurrentStudentCount--;
        if (CurrentStudentCount < MaxStudents)
            IsAcceptingNewStudents = true;
    }

    public void CloseForNewStudents() => IsAcceptingNewStudents = false;
    public void OpenForNewStudents() => IsAcceptingNewStudents = true;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
