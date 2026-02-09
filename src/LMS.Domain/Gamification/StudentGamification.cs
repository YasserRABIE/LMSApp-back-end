using LMS.Domain.Common;

namespace LMS.Domain.Gamification;

public sealed class StudentGamification : AggregateRoot<StudentGamificationId>
{
    public Guid StudentId { get; private set; }
    public int TotalXp { get; private set; }
    public Guid CurrentLevelId { get; private set; }
    public int PointsBalance { get; private set; }
    public int TotalPointsEarned { get; private set; }
    public int TotalPointsSpent { get; private set; }
    public short CurrentStreak { get; private set; }
    public short LongestStreak { get; private set; }
    public DateOnly? LastStreakDateUtc { get; private set; }
    public short StreakFreezeAvailable { get; private set; }
    public bool StreakFreezeUsedToday { get; private set; }

    private StudentGamification() : base(StudentGamificationId.New()) { }

    private StudentGamification(StudentGamificationId id, Guid studentId, Guid initialLevelId)
        : base(id) { StudentId = studentId; CurrentLevelId = initialLevelId; }

    public static Result<StudentGamification> Create(Guid studentId, Guid initialLevelId)
    {
        if (studentId == Guid.Empty || initialLevelId == Guid.Empty)
            return Result<StudentGamification>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<StudentGamification>.Success(new StudentGamification(StudentGamificationId.New(), studentId, initialLevelId));
    }

    public void AddXp(int amount) { TotalXp += amount; }
    public Result AddPoints(int amount) { if (amount < 0) return Result.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput)); PointsBalance += amount; TotalPointsEarned += amount; return Result.Success(); }
    public Result SpendPoints(int amount) { if (amount > PointsBalance) return Result.Failure(Error.Conflict("GAMIFICATION.INSUFFICIENT_POINTS")); PointsBalance -= amount; TotalPointsSpent += amount; return Result.Success(); }
    public void UpdateLevel(Guid newLevelId) => CurrentLevelId = newLevelId;
    public void UpdateStreak(short newStreak, DateOnly date) { CurrentStreak = newStreak; LastStreakDateUtc = date; if (newStreak > LongestStreak) LongestStreak = newStreak; }
    public void BreakStreak() { CurrentStreak = 0; StreakFreezeUsedToday = false; }
    public Result UseStreakFreeze() { if (StreakFreezeAvailable == 0) return Result.Failure(Error.Conflict("GAMIFICATION.NO_FREEZE_AVAILABLE")); StreakFreezeAvailable--; StreakFreezeUsedToday = true; return Result.Success(); }
    public void AddStreakFreeze() => StreakFreezeAvailable++;
}
