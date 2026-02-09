namespace LMS.Domain.Assessment;

/// <summary>
/// Strongly-typed identifier for Assessment entity
/// </summary>
public readonly record struct AssessmentId(Guid Value)
{
    public static AssessmentId New() => new(Guid.NewGuid());
    public static AssessmentId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
