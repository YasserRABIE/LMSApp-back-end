using LMS.Domain.Gamification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ToTable("Level");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Number).IsRequired();
        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.NameEn).HasMaxLength(100);
        builder.Property(l => l.RequiredXp).IsRequired();
        builder.Property(l => l.BadgeImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(l => l.ShortMessage).HasMaxLength(200).IsRequired();
        builder.Property(l => l.LongMessage).HasMaxLength(1000).IsRequired();
        builder.Property(l => l.CreatedAtUtc).IsRequired();
        builder.Property(l => l.UpdatedAtUtc).IsRequired();
        builder.HasIndex(l => l.Number).IsUnique().HasDatabaseName("UQ_Level_Number");
        builder.HasIndex(l => l.RequiredXp).HasDatabaseName("IX_Level_RequiredXp");
    }
}

public sealed class StudentGamificationConfiguration : IEntityTypeConfiguration<StudentGamification>
{
    public void Configure(EntityTypeBuilder<StudentGamification> builder)
    {
        builder.ToTable("StudentGamification");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion(id => id.Value, value => StudentGamificationId.From(value)).ValueGeneratedNever();
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.TotalXp).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.CurrentLevelId).IsRequired();
        builder.Property(s => s.PointsBalance).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.TotalPointsEarned).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.TotalPointsSpent).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.CurrentStreak).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.LongestStreak).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.LastStreakDateUtc);
        builder.Property(s => s.StreakFreezeAvailable).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.StreakFreezeUsedToday).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => s.StudentId).IsUnique().HasDatabaseName("UQ_StudentGamification_Student");
        builder.HasIndex(s => s.TotalXp).HasDatabaseName("IX_StudentGamification_TotalXp");
        builder.Ignore(s => s.DomainEvents);
    }
}

public sealed class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievement");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Code).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.NameEn).HasMaxLength(200);
        builder.Property(a => a.Description).HasMaxLength(500).IsRequired();
        builder.Property(a => a.ShortMessage).HasMaxLength(200).IsRequired();
        builder.Property(a => a.LongMessage).HasMaxLength(1000).IsRequired();
        builder.Property(a => a.BadgeImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(a => a.BadgeImageLockedUrl).HasMaxLength(500);
        builder.Property(a => a.XpReward).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.PointsReward).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.CriteriaType).HasConversion<int>().IsRequired();
        builder.Property(a => a.CriteriaValue);
        builder.Property(a => a.IsRepeatable).IsRequired().HasDefaultValue(false);
        builder.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.DisplayOrder).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.CreatedAtUtc).IsRequired();
        builder.Property(a => a.UpdatedAtUtc).IsRequired();
        builder.HasIndex(a => a.Code).IsUnique().HasDatabaseName("UQ_Achievement_Code");
        builder.HasIndex(a => a.DisplayOrder).HasDatabaseName("IX_Achievement_DisplayOrder");
    }
}

public sealed class StudentAchievementConfiguration : IEntityTypeConfiguration<StudentAchievement>
{
    public void Configure(EntityTypeBuilder<StudentAchievement> builder)
    {
        builder.ToTable("StudentAchievement");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.AchievementId).IsRequired();
        builder.Property(s => s.EarnedAtUtc).IsRequired();
        builder.Property(s => s.EarnedCount).IsRequired().HasDefaultValue(1);
        builder.Property(s => s.CurrentProgress);
        builder.Property(s => s.TargetProgress);
        builder.Property(s => s.IsNotified).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.NotifiedAtUtc);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.AchievementId }).HasDatabaseName("IX_StudentAchievement_Student_Achievement");
        builder.HasIndex(s => s.StudentId).HasDatabaseName("IX_StudentAchievement_Student");
    }
}

public sealed class XpTransactionConfiguration : IEntityTypeConfiguration<XpTransaction>
{
    public void Configure(EntityTypeBuilder<XpTransaction> builder)
    {
        builder.ToTable("XpTransaction");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StudentId).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.Source).HasConversion<int>().IsRequired();
        builder.Property(x => x.SourceEntityType).HasMaxLength(100);
        builder.Property(x => x.SourceEntityId);
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.XpBefore).IsRequired();
        builder.Property(x => x.XpAfter).IsRequired();
        builder.Property(x => x.LevelUpTriggered).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.NewLevelId);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.HasIndex(x => new { x.StudentId, x.CreatedAtUtc }).HasDatabaseName("IX_XpTransaction_Student");
        builder.HasIndex(x => new { x.SourceEntityType, x.SourceEntityId }).HasDatabaseName("IX_XpTransaction_Source");
    }
}

public sealed class PointsTransactionConfiguration : IEntityTypeConfiguration<PointsTransaction>
{
    public void Configure(EntityTypeBuilder<PointsTransaction> builder)
    {
        builder.ToTable("PointsTransaction");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.StudentId).IsRequired();
        builder.Property(p => p.Amount).IsRequired();
        builder.Property(p => p.TransactionType).HasConversion<int>().IsRequired();
        builder.Property(p => p.Source).IsRequired();
        builder.Property(p => p.SourceEntityType).HasMaxLength(100);
        builder.Property(p => p.SourceEntityId);
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();
        builder.Property(p => p.BalanceBefore).IsRequired();
        builder.Property(p => p.BalanceAfter).IsRequired();
        builder.Property(p => p.RelatedOrderId);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.HasIndex(p => new { p.StudentId, p.CreatedAtUtc }).HasDatabaseName("IX_PointsTransaction_Student");
        builder.HasIndex(p => new { p.SourceEntityType, p.SourceEntityId }).HasDatabaseName("IX_PointsTransaction_Source");
    }
}
