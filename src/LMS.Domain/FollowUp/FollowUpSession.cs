using LMS.Domain.Common;

namespace LMS.Domain.FollowUp;

public sealed class FollowUpSession : Entity<Guid>
{
    public Guid StudentFollowUpEnrollmentId { get; private set; }
    public Guid AssistantId { get; private set; }
    public TriggerType TriggerType { get; private set; }
    public Guid? TriggerStageId { get; private set; }
    public DateTime? ScheduledAtUtc { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? EndedAtUtc { get; private set; }
    public short? DurationMinutes { get; private set; }
    public short CallAttempts { get; private set; }
    public DateTime? LastCallAttemptAtUtc { get; private set; }
    public CallStatus CallStatus { get; private set; }
    public string? CallNotes { get; private set; }

    private FollowUpSession() : base() { }

    private FollowUpSession(Guid id, Guid enrollmentId, Guid assistantId, TriggerType triggerType, Guid? triggerStageId = null)
        : base(id)
    {
        StudentFollowUpEnrollmentId = enrollmentId;
        AssistantId = assistantId;
        TriggerType = triggerType;
        TriggerStageId = triggerStageId;
        CallStatus = CallStatus.Pending;
    }

    public static Result<FollowUpSession> Create(Guid enrollmentId, Guid assistantId, TriggerType triggerType, Guid? triggerStageId = null)
    {
        if (enrollmentId == Guid.Empty || assistantId == Guid.Empty)
            return Result<FollowUpSession>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<FollowUpSession>.Success(new FollowUpSession(Guid.NewGuid(), enrollmentId, assistantId, triggerType, triggerStageId));
    }

    public void Schedule(DateTime scheduledAtUtc) { CallStatus = CallStatus.Scheduled; ScheduledAtUtc = scheduledAtUtc; }
    public void Start() { CallStatus = CallStatus.InProgress; StartedAtUtc = DateTime.UtcNow; }
    public void Complete(short durationMinutes, string? notes = null) { CallStatus = CallStatus.Completed; EndedAtUtc = DateTime.UtcNow; DurationMinutes = durationMinutes; CallNotes = notes; }
    public void RecordAttempt() { CallAttempts++; LastCallAttemptAtUtc = DateTime.UtcNow; }
    public void MarkAsNoAnswer() { CallStatus = CallStatus.NoAnswer; RecordAttempt(); }
    public void Reschedule(DateTime newScheduledAtUtc) { CallStatus = CallStatus.Rescheduled; ScheduledAtUtc = newScheduledAtUtc; }
    public void UpdateNotes(string? notes) => CallNotes = notes;
}
