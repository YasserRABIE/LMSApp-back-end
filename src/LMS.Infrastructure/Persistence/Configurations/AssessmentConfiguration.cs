using LMS.Domain.Assessment;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class QuestionBankConfiguration : IEntityTypeConfiguration<QuestionBank>
{
    public void Configure(EntityTypeBuilder<QuestionBank> builder)
    {
        builder.ToTable("QuestionBank");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(q => q.Name).HasMaxLength(200).IsRequired();
        builder.Property(q => q.Description).HasColumnType("text");
        builder.Property(q => q.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(q => q.CreatedAtUtc).IsRequired();
        builder.Property(q => q.UpdatedAtUtc).IsRequired();
    }
}

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Question");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.QuestionBankId).IsRequired();
        builder.Property(q => q.QuestionType).HasConversion<int>().IsRequired();
        builder.Property(q => q.QuestionText).HasColumnType("text").IsRequired();
        builder.Property(q => q.QuestionImageUrl).HasMaxLength(500);
        builder.Property(q => q.Difficulty).HasConversion<int>().IsRequired();
        builder.Property(q => q.DefaultMarks).HasPrecision(5, 2).IsRequired();
        builder.Property(q => q.Explanation).HasColumnType("text");
        builder.Property(q => q.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(q => q.CreatedAtUtc).IsRequired();
        builder.Property(q => q.UpdatedAtUtc).IsRequired();
        builder.HasIndex(q => q.QuestionBankId).HasDatabaseName("IX_Question_Bank");
    }
}

public sealed class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOption");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.QuestionId).IsRequired();
        builder.Property(q => q.OptionText).HasColumnType("text").IsRequired();
        builder.Property(q => q.OptionImageUrl).HasMaxLength(500);
        builder.Property(q => q.IsCorrect).IsRequired();
        builder.Property(q => q.DisplayOrder).IsRequired();
        builder.Property(q => q.CreatedAtUtc).IsRequired();
        builder.HasIndex(q => new { q.QuestionId, q.DisplayOrder }).HasDatabaseName("IX_QuestionOption_Question");
    }
}

public sealed class QuestionTagConfiguration : IEntityTypeConfiguration<QuestionTag>
{
    public void Configure(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.ToTable("QuestionTag");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.QuestionId).IsRequired();
        builder.Property(q => q.Tag).HasMaxLength(50).IsRequired();
        builder.HasIndex(q => new { q.QuestionId, q.Tag }).IsUnique().HasDatabaseName("UQ_QuestionTag");
        builder.HasIndex(q => q.Tag).HasDatabaseName("IX_QuestionTag_Tag");
    }
}

public sealed class GradingRubricConfiguration : IEntityTypeConfiguration<GradingRubric>
{
    public void Configure(EntityTypeBuilder<GradingRubric> builder)
    {
        builder.ToTable("GradingRubric");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).HasMaxLength(200).IsRequired();
        builder.Property(g => g.Description).HasColumnType("text");
        builder.Property(g => g.IsDefault).IsRequired().HasDefaultValue(false);
        builder.Property(g => g.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(g => g.CreatedAtUtc).IsRequired();
        builder.Property(g => g.UpdatedAtUtc).IsRequired();
    }
}

public sealed class RubricCriteriaConfiguration : IEntityTypeConfiguration<RubricCriteria>
{
    public void Configure(EntityTypeBuilder<RubricCriteria> builder)
    {
        builder.ToTable("RubricCriteria");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RubricId).IsRequired();
        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Description).HasColumnType("text");
        builder.Property(r => r.MaxScore).HasPrecision(5, 2).IsRequired();
        builder.Property(r => r.DisplayOrder).IsRequired();
        builder.Property(r => r.CreatedAtUtc).IsRequired();
    }
}

public sealed class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("Assessment");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasConversion(id => id.Value, value => AssessmentId.From(value)).ValueGeneratedNever();
        builder.Property(a => a.ContentItemId);
        builder.Property(a => a.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(a => a.ModuleId);
        builder.Property(a => a.AssessmentType).HasConversion<int>().IsRequired();
        builder.Property(a => a.Title).HasMaxLength(300).IsRequired();
        builder.Property(a => a.Instructions).HasColumnType("text");
        builder.Property(a => a.TotalMarks).HasPrecision(7, 2).IsRequired();
        builder.Property(a => a.PassingMarks).HasPrecision(7, 2).IsRequired();
        builder.Property(a => a.DurationMinutes);
        builder.Property(a => a.MaxRetakes).IsRequired().HasDefaultValue(5);
        builder.Property(a => a.RetakeCooldownMinutes).IsRequired().HasDefaultValue(5);
        builder.Property(a => a.ShuffleQuestions).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.ShuffleOptions).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.ShowCorrectAnswers).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.AllowLateSubmission).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.AvailableFromUtc);
        builder.Property(a => a.DeadlineUtc);
        builder.Property(a => a.XpReward).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.PointsReward).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.XpRewardIfLate).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.PointsRewardIfLate).IsRequired().HasDefaultValue(0);
        builder.Property(a => a.GradingRubricId);
        builder.Property(a => a.Visibility).HasConversion<int>().IsRequired();
        builder.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.CreatedAtUtc).IsRequired();
        builder.Property(a => a.UpdatedAtUtc).IsRequired();
        builder.HasIndex(a => a.ContentItemId).HasDatabaseName("IX_Assessment_Content");
        builder.HasIndex(a => new { a.CourseId, a.ModuleId }).HasDatabaseName("IX_Assessment_Course_Module");
        builder.HasIndex(a => a.AssessmentType).HasDatabaseName("IX_Assessment_Type");
        builder.Ignore(a => a.DomainEvents);
    }
}

public sealed class AssessmentQuestionConfiguration : IEntityTypeConfiguration<AssessmentQuestion>
{
    public void Configure(EntityTypeBuilder<AssessmentQuestion> builder)
    {
        builder.ToTable("AssessmentQuestion");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AssessmentId).HasConversion(id => id.Value, value => AssessmentId.From(value)).IsRequired();
        builder.Property(a => a.QuestionId).IsRequired();
        builder.Property(a => a.Marks).HasPrecision(5, 2).IsRequired();
        builder.Property(a => a.DisplayOrder).IsRequired();
        builder.HasIndex(a => new { a.AssessmentId, a.QuestionId }).IsUnique().HasDatabaseName("UQ_AssessmentQuestion");
        builder.HasIndex(a => new { a.AssessmentId, a.DisplayOrder }).HasDatabaseName("IX_AssessmentQuestion_Assessment");
    }
}

public sealed class StudentAttemptConfiguration : IEntityTypeConfiguration<StudentAttempt>
{
    public void Configure(EntityTypeBuilder<StudentAttempt> builder)
    {
        builder.ToTable("StudentAttempt");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.AssessmentId).HasConversion(id => id.Value, value => AssessmentId.From(value)).IsRequired();
        builder.Property(s => s.AttemptNumber).IsRequired();
        builder.Property(s => s.Status).HasConversion<int>().IsRequired();
        builder.Property(s => s.StartedAtUtc).IsRequired();
        builder.Property(s => s.SubmittedAtUtc);
        builder.Property(s => s.DueAtUtc);
        builder.Property(s => s.IsLateSubmission).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.TotalScore).HasPrecision(7, 2);
        builder.Property(s => s.PercentageScore).HasPrecision(5, 2);
        builder.Property(s => s.IsPassed);
        builder.Property(s => s.GradedByAssistantId);
        builder.Property(s => s.GradedAtUtc);
        builder.Property(s => s.XpAwarded).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.PointsAwarded).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.GraderNotes).HasColumnType("text");
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.AssessmentId, s.AttemptNumber }).IsUnique().HasDatabaseName("UQ_StudentAttempt");
        builder.HasIndex(s => new { s.StudentId, s.Status }).HasDatabaseName("IX_StudentAttempt_Student");
        builder.HasIndex(s => s.AssessmentId).HasDatabaseName("IX_StudentAttempt_Assessment");
    }
}

public sealed class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
{
    public void Configure(EntityTypeBuilder<StudentAnswer> builder)
    {
        builder.ToTable("StudentAnswer");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.AttemptId).IsRequired();
        builder.Property(s => s.QuestionId).IsRequired();
        builder.Property(s => s.SelectedOptionId);
        builder.Property(s => s.BooleanAnswer);
        builder.Property(s => s.UploadedFileUrl).HasMaxLength(1000);
        builder.Property(s => s.UploadedFileName).HasMaxLength(300);
        builder.Property(s => s.IsCorrect);
        builder.Property(s => s.Score).HasPrecision(5, 2);
        builder.Property(s => s.Feedback).HasColumnType("text");
        builder.Property(s => s.AnsweredAtUtc).IsRequired();
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.AttemptId, s.QuestionId }).IsUnique().HasDatabaseName("UQ_StudentAnswer");
    }
}

public sealed class AttemptRubricScoreConfiguration : IEntityTypeConfiguration<AttemptRubricScore>
{
    public void Configure(EntityTypeBuilder<AttemptRubricScore> builder)
    {
        builder.ToTable("AttemptRubricScore");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AttemptId).IsRequired();
        builder.Property(a => a.CriteriaId).IsRequired();
        builder.Property(a => a.Score).HasPrecision(5, 2).IsRequired();
        builder.Property(a => a.Notes).HasColumnType("text");
        builder.Property(a => a.CreatedAtUtc).IsRequired();
        builder.Property(a => a.UpdatedAtUtc).IsRequired();
        builder.HasIndex(a => new { a.AttemptId, a.CriteriaId }).IsUnique().HasDatabaseName("UQ_AttemptRubricScore");
    }
}
