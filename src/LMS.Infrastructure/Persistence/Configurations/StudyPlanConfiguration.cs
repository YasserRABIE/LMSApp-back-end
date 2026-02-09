using LMS.Domain.StudyPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class StudyPlanSettingsConfiguration : IEntityTypeConfiguration<StudyPlanSettings>
{
    public void Configure(EntityTypeBuilder<StudyPlanSettings> builder)
    {
        builder.ToTable("StudyPlanSettings");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.ModuleId).IsRequired();
        builder.Property(s => s.DailyStudyMinutes).IsRequired();
        builder.Property(s => s.PreferredStartTime);
        builder.Property(s => s.IncludeReviewDays).IsRequired().HasDefaultValue(true);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.ModuleId }).IsUnique().HasDatabaseName("UQ_StudyPlanSettings");
    }
}

public sealed class StudyPlanStudyDayConfiguration : IEntityTypeConfiguration<StudyPlanStudyDay>
{
    public void Configure(EntityTypeBuilder<StudyPlanStudyDay> builder)
    {
        builder.ToTable("StudyPlanStudyDay");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SettingsId).IsRequired();
        builder.Property(s => s.DayOfWeek).IsRequired();
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.SettingsId, s.DayOfWeek }).IsUnique().HasDatabaseName("UQ_StudyPlanStudyDay");
    }
}

public sealed class StudyPlanConfiguration : IEntityTypeConfiguration<StudyPlan>
{
    public void Configure(EntityTypeBuilder<StudyPlan> builder)
    {
        builder.ToTable("StudyPlan");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion(id => id.Value, value => StudyPlanId.From(value)).ValueGeneratedNever();
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.ModuleId).IsRequired();
        builder.Property(s => s.SettingsId).IsRequired();
        builder.Property(s => s.StartDate).IsRequired();
        builder.Property(s => s.EndDate).IsRequired();
        builder.Property(s => s.ModuleReleaseDate).IsRequired();
        builder.Property(s => s.IsLatePurchase).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.Status).HasConversion<int>().IsRequired();
        builder.Property(s => s.CompletionPercent).HasPrecision(5, 2).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.TotalPlannedTasks).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.CompletedTasks).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.MissedTasks).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.RescheduledCount).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.GeneratedAtUtc).IsRequired();
        builder.Property(s => s.LastAdjustedAtUtc);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.ModuleId }).HasDatabaseName("IX_StudyPlan_Student_Module");
        builder.HasIndex(s => new { s.StudentId, s.Status }).HasDatabaseName("IX_StudyPlan_Student_Status");
        builder.Ignore(s => s.DomainEvents);
    }
}

public sealed class PlanDayConfiguration : IEntityTypeConfiguration<PlanDay>
{
    public void Configure(EntityTypeBuilder<PlanDay> builder)
    {
        builder.ToTable("PlanDay");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.StudyPlanId).HasConversion(id => id.Value, value => StudyPlanId.From(value)).IsRequired();
        builder.Property(p => p.Date).IsRequired();
        builder.Property(p => p.DayType).HasConversion<int>().IsRequired();
        builder.Property(p => p.Status).HasConversion<int>().IsRequired();
        builder.Property(p => p.PlannedMinutes).IsRequired();
        builder.Property(p => p.ActualMinutes);
        builder.Property(p => p.Notes).HasMaxLength(500);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.HasIndex(p => new { p.StudyPlanId, p.Date }).IsUnique().HasDatabaseName("UQ_PlanDay");
        builder.HasIndex(p => new { p.StudyPlanId, p.Status }).HasDatabaseName("IX_PlanDay_StudyPlan_Status");
    }
}

public sealed class PlanTaskConfiguration : IEntityTypeConfiguration<PlanTask>
{
    public void Configure(EntityTypeBuilder<PlanTask> builder)
    {
        builder.ToTable("PlanTask");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PlanDayId).IsRequired();
        builder.Property(p => p.ContentItemId);
        builder.Property(p => p.TaskType).HasConversion<int>().IsRequired();
        builder.Property(p => p.Title).HasMaxLength(300).IsRequired();
        builder.Property(p => p.Description).HasColumnType("text");
        builder.Property(p => p.EstimatedMinutes).IsRequired();
        builder.Property(p => p.DisplayOrder).IsRequired();
        builder.Property(p => p.Status).HasConversion<int>().IsRequired();
        builder.Property(p => p.CompletedAtUtc);
        builder.Property(p => p.ActualMinutes);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.HasIndex(p => new { p.PlanDayId, p.DisplayOrder }).HasDatabaseName("IX_PlanTask_Day");
        builder.HasIndex(p => p.ContentItemId).HasDatabaseName("IX_PlanTask_Content");
    }
}

public sealed class ReviewContentConfiguration : IEntityTypeConfiguration<ReviewContent>
{
    public void Configure(EntityTypeBuilder<ReviewContent> builder)
    {
        builder.ToTable("ReviewContent");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PlanTaskId).IsRequired();
        builder.Property(r => r.ContentItemId).IsRequired();
        builder.Property(r => r.DisplayOrder).IsRequired();
        builder.Property(r => r.IsCompleted).IsRequired().HasDefaultValue(false);
        builder.Property(r => r.CompletedAtUtc);
        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.UpdatedAtUtc).IsRequired();
        builder.HasIndex(r => new { r.PlanTaskId, r.ContentItemId }).IsUnique().HasDatabaseName("UQ_ReviewContent");
    }
}

public sealed class ReviewQuizConfiguration : IEntityTypeConfiguration<ReviewQuiz>
{
    public void Configure(EntityTypeBuilder<ReviewQuiz> builder)
    {
        builder.ToTable("ReviewQuiz");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PlanTaskId).IsRequired();
        builder.Property(r => r.StageId).IsRequired();
        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.UpdatedAtUtc).IsRequired();
        builder.HasIndex(r => r.PlanTaskId).IsUnique().HasDatabaseName("UQ_ReviewQuiz_Task");
        builder.HasIndex(r => r.StageId).HasDatabaseName("IX_ReviewQuiz_Stage");
    }
}

public sealed class ReviewQuizQuestionConfiguration : IEntityTypeConfiguration<ReviewQuizQuestion>
{
    public void Configure(EntityTypeBuilder<ReviewQuizQuestion> builder)
    {
        builder.ToTable("ReviewQuizQuestion");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ReviewQuizId).IsRequired();
        builder.Property(r => r.QuestionId).IsRequired();
        builder.Property(r => r.DisplayOrder).IsRequired();
        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.UpdatedAtUtc).IsRequired();
        builder.HasIndex(r => new { r.ReviewQuizId, r.QuestionId }).IsUnique().HasDatabaseName("UQ_ReviewQuizQuestion");
        builder.HasIndex(r => new { r.ReviewQuizId, r.DisplayOrder }).HasDatabaseName("IX_ReviewQuizQuestion_Quiz");
    }
}

public sealed class PlanAdjustmentLogConfiguration : IEntityTypeConfiguration<PlanAdjustmentLog>
{
    public void Configure(EntityTypeBuilder<PlanAdjustmentLog> builder)
    {
        builder.ToTable("PlanAdjustmentLog");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.StudyPlanId).HasConversion(id => id.Value, value => StudyPlanId.From(value)).IsRequired();
        builder.Property(p => p.AdjustmentType).HasConversion<int>().IsRequired();
        builder.Property(p => p.Reason).HasMaxLength(500).IsRequired();
        builder.Property(p => p.AdjustedByUserId);
        builder.Property(p => p.TasksRescheduled).IsRequired().HasDefaultValue(0);
        builder.Property(p => p.ReviewTasksAdded).IsRequired().HasDefaultValue(0);
        builder.Property(p => p.PreviousEndDate);
        builder.Property(p => p.NewEndDate);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.HasIndex(p => p.StudyPlanId).HasDatabaseName("IX_PlanAdjustmentLog_StudyPlan");
    }
}
