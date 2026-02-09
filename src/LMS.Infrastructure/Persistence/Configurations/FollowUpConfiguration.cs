using LMS.Domain.Content;
using LMS.Domain.FollowUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class FollowUpGroupConfiguration : IEntityTypeConfiguration<FollowUpGroup>
{
    public void Configure(EntityTypeBuilder<FollowUpGroup> builder)
    {
        builder.ToTable("FollowUpGroup");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasConversion(id => id.Value, value => FollowUpGroupId.From(value)).ValueGeneratedNever();
        builder.Property(f => f.Name).HasMaxLength(200).IsRequired();
        builder.Property(f => f.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(f => f.ModuleId).IsRequired();
        builder.Property(f => f.StudyLevelTrackId).IsRequired();
        builder.Property(f => f.AssistantId).IsRequired();
        builder.Property(f => f.MaxStudents).IsRequired();
        builder.Property(f => f.CurrentStudentCount).IsRequired().HasDefaultValue(0);
        builder.Property(f => f.IsAcceptingNewStudents).IsRequired().HasDefaultValue(true);
        builder.Property(f => f.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(f => f.CreatedByUserId).IsRequired();
        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc).IsRequired();
        builder.HasIndex(f => new { f.CourseId, f.ModuleId, f.AssistantId }).HasDatabaseName("IX_FollowUpGroup_Course_Module_Assistant");
        builder.HasIndex(f => f.AssistantId).HasDatabaseName("IX_FollowUpGroup_Assistant");
        builder.Ignore(f => f.DomainEvents);
    }
}

public sealed class FollowUpWaitingListConfiguration : IEntityTypeConfiguration<FollowUpWaitingList>
{
    public void Configure(EntityTypeBuilder<FollowUpWaitingList> builder)
    {
        builder.ToTable("FollowUpWaitingList");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.StudentId).IsRequired();
        builder.Property(f => f.ModuleId).IsRequired();
        builder.Property(f => f.EnrollmentId).IsRequired();
        builder.Property(f => f.PreferredAssistantId);
        builder.Property(f => f.Position).IsRequired();
        builder.Property(f => f.Status).HasConversion<int>().IsRequired();
        builder.Property(f => f.AssignedGroupId).HasConversion(
            id => id.HasValue ? id.Value.Value : (Guid?)null,
            value => value.HasValue ? FollowUpGroupId.From(value.Value) : null);
        builder.Property(f => f.AssignedAtUtc);
        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc).IsRequired();
        builder.HasIndex(f => new { f.ModuleId, f.Status, f.Position }).HasDatabaseName("IX_FollowUpWaitingList_Module");
        builder.HasIndex(f => new { f.StudentId, f.ModuleId }).IsUnique().HasDatabaseName("UQ_FollowUpWaitingList");
    }
}

public sealed class StudentFollowUpEnrollmentConfiguration : IEntityTypeConfiguration<StudentFollowUpEnrollment>
{
    public void Configure(EntityTypeBuilder<StudentFollowUpEnrollment> builder)
    {
        builder.ToTable("StudentFollowUpEnrollment");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.GroupId).HasConversion(id => id.Value, value => FollowUpGroupId.From(value)).IsRequired();
        builder.Property(s => s.EnrollmentId).IsRequired();
        builder.Property(s => s.Status).HasConversion<int>().IsRequired();
        builder.Property(s => s.StartDateUtc).IsRequired();
        builder.Property(s => s.EndDateUtc);
        builder.Property(s => s.TotalSessionsScheduled).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.TotalSessionsCompleted).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.TotalSessionsMissed).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.GroupId }).IsUnique().HasDatabaseName("UQ_StudentFollowUpEnrollment");
        builder.HasIndex(s => new { s.StudentId, s.Status }).HasDatabaseName("IX_StudentFollowUpEnrollment_Student");
        builder.HasIndex(s => s.GroupId).HasDatabaseName("IX_StudentFollowUpEnrollment_Group");
    }
}

public sealed class StudentAssistantHistoryConfiguration : IEntityTypeConfiguration<StudentAssistantHistory>
{
    public void Configure(EntityTypeBuilder<StudentAssistantHistory> builder)
    {
        builder.ToTable("StudentAssistantHistory");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.AssistantId).IsRequired();
        builder.Property(s => s.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(s => s.ModuleId).IsRequired();
        builder.Property(s => s.GroupId).HasConversion(id => id.Value, value => FollowUpGroupId.From(value)).IsRequired();
        builder.Property(s => s.IsLatest).IsRequired().HasDefaultValue(true);
        builder.Property(s => s.AssignedAtUtc).IsRequired();
        builder.Property(s => s.EndedAtUtc);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.ModuleId, s.IsLatest }).HasDatabaseName("IX_StudentAssistantHistory_Student_Module");
        builder.HasIndex(s => s.AssistantId).HasDatabaseName("IX_StudentAssistantHistory_Assistant");
    }
}

public sealed class FollowUpSessionConfiguration : IEntityTypeConfiguration<FollowUpSession>
{
    public void Configure(EntityTypeBuilder<FollowUpSession> builder)
    {
        builder.ToTable("FollowUpSession");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.StudentFollowUpEnrollmentId).IsRequired();
        builder.Property(f => f.AssistantId).IsRequired();
        builder.Property(f => f.TriggerType).HasConversion<int>().IsRequired();
        builder.Property(f => f.TriggerStageId);
        builder.Property(f => f.ScheduledAtUtc);
        builder.Property(f => f.StartedAtUtc);
        builder.Property(f => f.EndedAtUtc);
        builder.Property(f => f.DurationMinutes);
        builder.Property(f => f.CallAttempts).IsRequired().HasDefaultValue(0);
        builder.Property(f => f.LastCallAttemptAtUtc);
        builder.Property(f => f.CallStatus).HasConversion<int>().IsRequired();
        builder.Property(f => f.CallNotes).HasColumnType("text");
        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc).IsRequired();
        builder.HasIndex(f => f.StudentFollowUpEnrollmentId).HasDatabaseName("IX_FollowUpSession_Enrollment");
        builder.HasIndex(f => new { f.AssistantId, f.CallStatus }).HasDatabaseName("IX_FollowUpSession_Assistant_Status");
    }
}

public sealed class FollowUpEvaluationConfiguration : IEntityTypeConfiguration<FollowUpEvaluation>
{
    public void Configure(EntityTypeBuilder<FollowUpEvaluation> builder)
    {
        builder.ToTable("FollowUpEvaluation");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.SessionId).IsRequired();
        builder.Property(f => f.CommitmentScore).IsRequired();
        builder.Property(f => f.UnderstandingScore).IsRequired();
        builder.Property(f => f.OverallScore).HasPrecision(3, 1).IsRequired();
        builder.Property(f => f.StudentStatus).HasConversion<int>().IsRequired();
        builder.Property(f => f.PrivateNotes).HasColumnType("text");
        builder.Property(f => f.ParentNotes).HasColumnType("text");
        builder.Property(f => f.RecommendedAction).HasConversion<int>().IsRequired();
        builder.Property(f => f.UnlockNextStage).IsRequired().HasDefaultValue(false);
        builder.Property(f => f.NextStageId);
        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc).IsRequired();
        builder.HasIndex(f => f.SessionId).IsUnique().HasDatabaseName("UQ_FollowUpEvaluation_Session");
    }
}
