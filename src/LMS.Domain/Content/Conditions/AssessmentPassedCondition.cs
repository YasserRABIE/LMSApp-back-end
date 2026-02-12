namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires passing an assessment (based on assessment's pass criteria)
/// </summary>
public sealed class AssessmentPassedCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.AssessmentPassed;
    public string AssessmentId { get; private set; }

    private AssessmentPassedCondition() : base()
    {
        AssessmentId = string.Empty;
    }

    private AssessmentPassedCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string assessmentId,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        AssessmentId = assessmentId;
    }

    public static AssessmentPassedCondition Create(
        PrerequisiteId prerequisiteId,
        string assessmentId,
        int displayOrder = 0)
    {
        return new AssessmentPassedCondition(
            ConditionId.New(),
            prerequisiteId,
            assessmentId,
            displayOrder);
    }
}
