namespace LMS.Domain.StudyPlan;

/// <summary>
/// Strongly-typed identifier for StudyPlan entity
/// </summary>
public readonly record struct StudyPlanId(Guid Value)
{
    public static StudyPlanId New() => new(Guid.NewGuid());
    public static StudyPlanId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
