using LMS.Domain.Common;

namespace LMS.Domain.FollowUp;

public sealed class StudentFollowUpEnrollment : Entity<Guid>
{
    public Guid StudentId { get; private set; }
    public FollowUpGroupId GroupId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public FollowUpEnrollmentStatus Status { get; private set; }
    public DateTime StartDateUtc { get; private set; }
    public DateTime? EndDateUtc { get; private set; }
    public short TotalSessionsScheduled { get; private set; }
    public short TotalSessionsCompleted { get; private set; }
    public short TotalSessionsMissed { get; private set; }

    private StudentFollowUpEnrollment() : base() { GroupId = FollowUpGroupId.New(); }

    private StudentFollowUpEnrollment(Guid id, Guid studentId, FollowUpGroupId groupId, Guid enrollmentId)
        : base(id)
    {
        StudentId = studentId;
        GroupId = groupId;
        EnrollmentId = enrollmentId;
        Status = FollowUpEnrollmentStatus.Active;
        StartDateUtc = DateTime.UtcNow;
    }

    public static Result<StudentFollowUpEnrollment> Create(Guid studentId, FollowUpGroupId groupId, Guid enrollmentId)
    {
        if (studentId == Guid.Empty || groupId.Value == Guid.Empty || enrollmentId == Guid.Empty)
            return Result<StudentFollowUpEnrollment>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<StudentFollowUpEnrollment>.Success(new StudentFollowUpEnrollment(Guid.NewGuid(), studentId, groupId, enrollmentId));
    }

    public void RecordScheduledSession() => TotalSessionsScheduled++;
    public void RecordCompletedSession() => TotalSessionsCompleted++;
    public void RecordMissedSession() => TotalSessionsMissed++;
    public void Pause() => Status = FollowUpEnrollmentStatus.Paused;
    public void Resume() => Status = FollowUpEnrollmentStatus.Active;
    public void Cancel() { Status = FollowUpEnrollmentStatus.Cancelled; EndDateUtc = DateTime.UtcNow; }
    public void Complete() { Status = FollowUpEnrollmentStatus.Completed; EndDateUtc = DateTime.UtcNow; }
}
