namespace LMS.Domain.Gamification;

public readonly record struct StudentGamificationId(Guid Value)
{
    public static StudentGamificationId New() => new(Guid.NewGuid());
    public static StudentGamificationId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
