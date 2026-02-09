using LMS.Domain.Assessment;
using LMS.Domain.Content;
using LMS.Domain.FollowUp;
using LMS.Domain.Gamification;
using LMS.Domain.Notification;
using LMS.Domain.Purchasing;
using LMS.Domain.StudyPlan;
using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence;

/// <summary>
/// Main database context for the LMS application
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // User Aggregate
    public DbSet<User> Users => Set<User>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<TeacherProfile> TeacherProfiles => Set<TeacherProfile>();
    public DbSet<AssistantProfile> AssistantProfiles => Set<AssistantProfile>();
    public DbSet<ParentProfile> ParentProfiles => Set<ParentProfile>();
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();

    // Reference Data
    public DbSet<StudyLevel> StudyLevels => Set<StudyLevel>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<StudyLevelTrack> StudyLevelTracks => Set<StudyLevelTrack>();

    // Content Aggregate
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<ContentItem> ContentItems => Set<ContentItem>();
    public DbSet<VideoContent> VideoContents => Set<VideoContent>();
    public DbSet<FileContent> FileContents => Set<FileContent>();
    public DbSet<ContentPrerequisite> ContentPrerequisites => Set<ContentPrerequisite>();
    public DbSet<StudentContentProgress> StudentContentProgresses => Set<StudentContentProgress>();

    // Assessment Aggregate
    public DbSet<QuestionBank> QuestionBanks => Set<QuestionBank>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<QuestionTag> QuestionTags => Set<QuestionTag>();
    public DbSet<GradingRubric> GradingRubrics => Set<GradingRubric>();
    public DbSet<RubricCriteria> RubricCriterias => Set<RubricCriteria>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<AssessmentQuestion> AssessmentQuestions => Set<AssessmentQuestion>();
    public DbSet<StudentAttempt> StudentAttempts => Set<StudentAttempt>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();
    public DbSet<AttemptRubricScore> AttemptRubricScores => Set<AttemptRubricScore>();

    // StudyPlan Aggregate
    public DbSet<StudyPlanSettings> StudyPlanSettings => Set<StudyPlanSettings>();
    public DbSet<StudyPlanStudyDay> StudyPlanStudyDays => Set<StudyPlanStudyDay>();
    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
    public DbSet<PlanDay> PlanDays => Set<PlanDay>();
    public DbSet<PlanTask> PlanTasks => Set<PlanTask>();
    public DbSet<ReviewContent> ReviewContents => Set<ReviewContent>();
    public DbSet<ReviewQuiz> ReviewQuizzes => Set<ReviewQuiz>();
    public DbSet<ReviewQuizQuestion> ReviewQuizQuestions => Set<ReviewQuizQuestion>();
    public DbSet<PlanAdjustmentLog> PlanAdjustmentLogs => Set<PlanAdjustmentLog>();

    // FollowUp Aggregate
    public DbSet<FollowUpGroup> FollowUpGroups => Set<FollowUpGroup>();
    public DbSet<FollowUpWaitingList> FollowUpWaitingLists => Set<FollowUpWaitingList>();
    public DbSet<StudentFollowUpEnrollment> StudentFollowUpEnrollments => Set<StudentFollowUpEnrollment>();
    public DbSet<StudentAssistantHistory> StudentAssistantHistories => Set<StudentAssistantHistory>();
    public DbSet<FollowUpSession> FollowUpSessions => Set<FollowUpSession>();
    public DbSet<FollowUpEvaluation> FollowUpEvaluations => Set<FollowUpEvaluation>();

    // Gamification Aggregate
    public DbSet<Level> Levels => Set<Level>();
    public DbSet<StudentGamification> StudentGamifications => Set<StudentGamification>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<StudentAchievement> StudentAchievements => Set<StudentAchievement>();
    public DbSet<XpTransaction> XpTransactions => Set<XpTransaction>();
    public DbSet<PointsTransaction> PointsTransactions => Set<PointsTransaction>();

    // Purchasing Aggregate
    public DbSet<Product> Products => Set<Product>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();

    // Notification Aggregate
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();
    public DbSet<UserNotificationSettings> UserNotificationSettings => Set<UserNotificationSettings>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Register PostgreSQL enums for better performance and type safety
        modelBuilder.HasPostgresEnum<UserType>();

        // Apply all entity configurations from the current assembly
        // (this also applies seed data from entity configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically update timestamps for modified entities
        UpdateTimestamps();

        // TODO: Add domain events dispatch here if needed

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically updates UpdatedAtUtc timestamp for modified entities
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Update UpdatedAtUtc for entities that have this property
            var updatedAtProperty = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "UpdatedAtUtc");

            if (updatedAtProperty != null)
            {
                updatedAtProperty.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
