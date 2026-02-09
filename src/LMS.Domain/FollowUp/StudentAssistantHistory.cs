using LMS.Domain.Common;
using LMS.Domain.Content;

namespace LMS.Domain.FollowUp;

public sealed class StudentAssistantHistory : Entity<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid AssistantId { get; private set; }
    public CourseId CourseId { get; private set; }
    public Guid ModuleId { get; private set; }
    public FollowUpGroupId GroupId { get; private set; }
    public bool IsLatest { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public DateTime? EndedAtUtc { get; private set; }

    private StudentAssistantHistory() : base() { CourseId = CourseId.New(); GroupId = FollowUpGroupId.New(); }

    private StudentAssistantHistory(Guid id, Guid studentId, Guid assistantId, CourseId courseId, Guid moduleId, FollowUpGroupId groupId)
        : base(id)
    {
        StudentId = studentId;
        AssistantId = assistantId;
        CourseId = courseId;
        ModuleId = moduleId;
        GroupId = groupId;
        IsLatest = true;
        AssignedAtUtc = DateTime.UtcNow;
    }

    public static Result<StudentAssistantHistory> Create(Guid studentId, Guid assistantId, CourseId courseId, Guid moduleId, FollowUpGroupId groupId)
    {
        if (studentId == Guid.Empty || assistantId == Guid.Empty || courseId.Value == Guid.Empty || moduleId == Guid.Empty || groupId.Value == Guid.Empty)
            return Result<StudentAssistantHistory>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<StudentAssistantHistory>.Success(new StudentAssistantHistory(Guid.NewGuid(), studentId, assistantId, courseId, moduleId, groupId));
    }

    public void MarkAsNotLatest() { IsLatest = false; EndedAtUtc = DateTime.UtcNow; }
}
