using LMS.Domain.Common;

namespace LMS.Domain.Gamification;

public sealed class Achievement : Entity<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? NameEn { get; private set; }
    public string Description { get; private set; }
    public string ShortMessage { get; private set; }
    public string LongMessage { get; private set; }
    public string BadgeImageUrl { get; private set; }
    public string? BadgeImageLockedUrl { get; private set; }
    public short XpReward { get; private set; }
    public short PointsReward { get; private set; }
    public CriteriaType CriteriaType { get; private set; }
    public int? CriteriaValue { get; private set; }
    public bool IsRepeatable { get; private set; }
    public bool IsActive { get; private set; }
    public short DisplayOrder { get; private set; }

    private Achievement() : base() { Code = string.Empty; Name = string.Empty; Description = string.Empty; ShortMessage = string.Empty; LongMessage = string.Empty; BadgeImageUrl = string.Empty; }

    public static Result<Achievement> Create(string code, string name, string description, string shortMessage, string longMessage, string badgeImageUrl, CriteriaType criteriaType, short xpReward = 0, short pointsReward = 0, int? criteriaValue = null, bool isRepeatable = false, short displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(code)) return Result<Achievement>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<Achievement>.Success(new Achievement { Id = Guid.NewGuid(), Code = code, Name = name, Description = description, ShortMessage = shortMessage, LongMessage = longMessage, BadgeImageUrl = badgeImageUrl, CriteriaType = criteriaType, XpReward = xpReward, PointsReward = pointsReward, CriteriaValue = criteriaValue, IsRepeatable = isRepeatable, IsActive = true, DisplayOrder = displayOrder });
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public sealed class StudentAchievement : Entity<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid AchievementId { get; private set; }
    public DateTime EarnedAtUtc { get; private set; }
    public short EarnedCount { get; private set; }
    public int? CurrentProgress { get; private set; }
    public int? TargetProgress { get; private set; }
    public bool IsNotified { get; private set; }
    public DateTime? NotifiedAtUtc { get; private set; }

    private StudentAchievement() : base() { }

    public static Result<StudentAchievement> Create(Guid studentId, Guid achievementId)
    {
        if (studentId == Guid.Empty || achievementId == Guid.Empty)
            return Result<StudentAchievement>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<StudentAchievement>.Success(new StudentAchievement { Id = Guid.NewGuid(), StudentId = studentId, AchievementId = achievementId, EarnedAtUtc = DateTime.UtcNow, EarnedCount = 1 });
    }

    public void IncrementEarnedCount() => EarnedCount++;
    public void UpdateProgress(int current, int target) { CurrentProgress = current; TargetProgress = target; }
    public void MarkAsNotified() { IsNotified = true; NotifiedAtUtc = DateTime.UtcNow; }
}

public sealed class XpTransaction : Entity<long>
{
    public Guid StudentId { get; private set; }
    public int Amount { get; private set; }
    public XpSource Source { get; private set; }
    public string? SourceEntityType { get; private set; }
    public Guid? SourceEntityId { get; private set; }
    public string Description { get; private set; }
    public int XpBefore { get; private set; }
    public int XpAfter { get; private set; }
    public bool LevelUpTriggered { get; private set; }
    public Guid? NewLevelId { get; private set; }

    private XpTransaction() : base() { Description = string.Empty; }

    public static Result<XpTransaction> Create(Guid studentId, int amount, XpSource source, string description, int xpBefore)
    {
        if (studentId == Guid.Empty || amount <= 0)
            return Result<XpTransaction>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<XpTransaction>.Success(new XpTransaction { StudentId = studentId, Amount = amount, Source = source, Description = description, XpBefore = xpBefore, XpAfter = xpBefore + amount });
    }

    public void TriggerLevelUp(Guid newLevelId) { LevelUpTriggered = true; NewLevelId = newLevelId; }
}

public sealed class PointsTransaction : Entity<long>
{
    public Guid StudentId { get; private set; }
    public int Amount { get; private set; }
    public PointsTransactionType TransactionType { get; private set; }
    public short Source { get; private set; }
    public string? SourceEntityType { get; private set; }
    public Guid? SourceEntityId { get; private set; }
    public string Description { get; private set; }
    public int BalanceBefore { get; private set; }
    public int BalanceAfter { get; private set; }
    public Guid? RelatedOrderId { get; private set; }

    private PointsTransaction() : base() { Description = string.Empty; }

    public static Result<PointsTransaction> Create(Guid studentId, int amount, PointsTransactionType type, short source, string description, int balanceBefore)
    {
        if (studentId == Guid.Empty) return Result<PointsTransaction>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<PointsTransaction>.Success(new PointsTransaction { StudentId = studentId, Amount = amount, TransactionType = type, Source = source, Description = description, BalanceBefore = balanceBefore, BalanceAfter = type == PointsTransactionType.Spent ? balanceBefore - amount : balanceBefore + amount });
    }
}
