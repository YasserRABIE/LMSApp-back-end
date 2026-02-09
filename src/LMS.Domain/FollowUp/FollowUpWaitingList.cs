using LMS.Domain.Common;

namespace LMS.Domain.FollowUp;

public sealed class FollowUpWaitingList : Entity<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid ModuleId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public Guid? PreferredAssistantId { get; private set; }
    public short Position { get; private set; }
    public WaitingListStatus Status { get; private set; }
    public FollowUpGroupId? AssignedGroupId { get; private set; }
    public DateTime? AssignedAtUtc { get; private set; }

    private FollowUpWaitingList() : base() { }

    private FollowUpWaitingList(Guid id, Guid studentId, Guid moduleId, Guid enrollmentId, short position, Guid? preferredAssistantId = null)
        : base(id)
    {
        StudentId = studentId;
        ModuleId = moduleId;
        EnrollmentId = enrollmentId;
        PreferredAssistantId = preferredAssistantId;
        Position = position;
        Status = WaitingListStatus.Waiting;
    }

    public static Result<FollowUpWaitingList> Create(Guid studentId, Guid moduleId, Guid enrollmentId, short position, Guid? preferredAssistantId = null)
    {
        if (studentId == Guid.Empty || moduleId == Guid.Empty || enrollmentId == Guid.Empty)
            return Result<FollowUpWaitingList>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        if (position < 0)
            return Result<FollowUpWaitingList>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        return Result<FollowUpWaitingList>.Success(new FollowUpWaitingList(Guid.NewGuid(), studentId, moduleId, enrollmentId, position, preferredAssistantId));
    }

    public Result AssignToGroup(FollowUpGroupId groupId)
    {
        if (Status != WaitingListStatus.Waiting)
            return Result.Failure(Error.Conflict("WAITING_LIST.INVALID_STATUS"));
        Status = WaitingListStatus.Assigned;
        AssignedGroupId = groupId;
        AssignedAtUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == WaitingListStatus.Assigned)
            return Result.Failure(Error.Conflict("WAITING_LIST.ALREADY_ASSIGNED"));
        Status = WaitingListStatus.Cancelled;
        return Result.Success();
    }
}
