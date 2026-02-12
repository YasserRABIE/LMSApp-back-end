namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires user to be in a specific academic track
/// </summary>
public sealed class UserTrackCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.UserTrack;
    public string RequiredTrackId { get; private set; }

    private UserTrackCondition() : base()
    {
        RequiredTrackId = string.Empty;
    }

    private UserTrackCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string requiredTrackId,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        RequiredTrackId = requiredTrackId;
    }

    public static UserTrackCondition Create(
        PrerequisiteId prerequisiteId,
        string requiredTrackId,
        int displayOrder = 0)
    {
        return new UserTrackCondition(
            ConditionId.New(),
            prerequisiteId,
            requiredTrackId,
            displayOrder);
    }
}
