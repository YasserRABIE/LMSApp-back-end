namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Requires a specific score on an assessment
/// </summary>
public sealed class AssessmentScoreCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.AssessmentScore;
    public string AssessmentId { get; private set; }
    public decimal MinScore { get; private set; }
    public ScoreType ScoreType { get; private set; }

    private AssessmentScoreCondition() : base()
    {
        AssessmentId = string.Empty;
    }

    private AssessmentScoreCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        string assessmentId,
        decimal minScore,
        ScoreType scoreType,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        AssessmentId = assessmentId;
        MinScore = minScore;
        ScoreType = scoreType;
    }

    public static AssessmentScoreCondition Create(
        PrerequisiteId prerequisiteId,
        string assessmentId,
        decimal minScore,
        ScoreType scoreType = ScoreType.Percentage,
        int displayOrder = 0)
    {
        return new AssessmentScoreCondition(
            ConditionId.New(),
            prerequisiteId,
            assessmentId,
            minScore,
            scoreType,
            displayOrder);
    }
}

public enum ScoreType
{
    Percentage = 1,
    Points = 2
}
