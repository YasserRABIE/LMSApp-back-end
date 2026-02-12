using LMS.Domain.Assessment;
using LMS.Domain.Content;
using LMS.Domain.Content.Actions;
using LMS.Domain.Content.Conditions;
using LMS.Domain.FollowUp;
using LMS.Domain.Gamification;
using LMS.Domain.Notification;
using LMS.Domain.Purchasing;
using LMS.Domain.StudyPlan;
using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using StudyPlanTaskStatus = LMS.Domain.StudyPlan.TaskStatus;


namespace LMS.Infrastructure.Persistence;

public sealed class LmsDbContext : DbContext
{
    public LmsDbContext(DbContextOptions<LmsDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<TeacherProfile> TeacherProfiles => Set<TeacherProfile>();
    public DbSet<AssistantProfile> AssistantProfiles => Set<AssistantProfile>();
    public DbSet<ParentProfile> ParentProfiles => Set<ParentProfile>();
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();

    public DbSet<StudyLevel> StudyLevels => Set<StudyLevel>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<StudyLevelTrack> StudyLevelTracks => Set<StudyLevelTrack>();

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SchoolType> SchoolTypes => Set<SchoolType>();
    public DbSet<CourseCategory> CourseCategories => Set<CourseCategory>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<ContentItem> ContentItems => Set<ContentItem>();
    public DbSet<VideoContent> VideoContents => Set<VideoContent>();
    public DbSet<FileContent> FileContents => Set<FileContent>();
    public DbSet<Prerequisite> Prerequisites => Set<Prerequisite>();
    public DbSet<StudentContentProgress> StudentContentProgresses => Set<StudentContentProgress>();

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

    public DbSet<StudyPlanSettings> StudyPlanSettings => Set<StudyPlanSettings>();
    public DbSet<StudyPlanStudyDay> StudyPlanStudyDays => Set<StudyPlanStudyDay>();
    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
    public DbSet<PlanDay> PlanDays => Set<PlanDay>();
    public DbSet<PlanTask> PlanTasks => Set<PlanTask>();
    public DbSet<ReviewContent> ReviewContents => Set<ReviewContent>();
    public DbSet<ReviewQuiz> ReviewQuizzes => Set<ReviewQuiz>();
    public DbSet<ReviewQuizQuestion> ReviewQuizQuestions => Set<ReviewQuizQuestion>();
    public DbSet<PlanAdjustmentLog> PlanAdjustmentLogs => Set<PlanAdjustmentLog>();

    public DbSet<FollowUpGroup> FollowUpGroups => Set<FollowUpGroup>();
    public DbSet<FollowUpWaitingList> FollowUpWaitingLists => Set<FollowUpWaitingList>();
    public DbSet<StudentFollowUpEnrollment> StudentFollowUpEnrollments => Set<StudentFollowUpEnrollment>();
    public DbSet<StudentAssistantHistory> StudentAssistantHistories => Set<StudentAssistantHistory>();
    public DbSet<FollowUpSession> FollowUpSessions => Set<FollowUpSession>();
    public DbSet<FollowUpEvaluation> FollowUpEvaluations => Set<FollowUpEvaluation>();

    public DbSet<Level> Levels => Set<Level>();
    public DbSet<StudentGamification> StudentGamifications => Set<StudentGamification>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<StudentAchievement> StudentAchievements => Set<StudentAchievement>();
    public DbSet<XpTransaction> XpTransactions => Set<XpTransaction>();
    public DbSet<PointsTransaction> PointsTransactions => Set<PointsTransaction>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();

    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();
    public DbSet<UserNotificationSettings> UserNotificationSettings => Set<UserNotificationSettings>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresEnum<UserType>();
        modelBuilder.HasPostgresEnum<ParentRelationType>();
        modelBuilder.HasPostgresEnum<Visibility>();
        modelBuilder.HasPostgresEnum<ContentType>();
        modelBuilder.HasPostgresEnum<VideoStatus>();
        modelBuilder.HasPostgresEnum<ProgressStatus>();
        modelBuilder.HasPostgresEnum<QuestionType>();
        modelBuilder.HasPostgresEnum<Difficulty>();
        modelBuilder.HasPostgresEnum<AssessmentType>();
        modelBuilder.HasPostgresEnum<AttemptStatus>();
        modelBuilder.HasPostgresEnum<PlanStatus>();
        modelBuilder.HasPostgresEnum<DayType>();
        modelBuilder.HasPostgresEnum<DayStatus>();
        modelBuilder.HasPostgresEnum<TaskType>();
        modelBuilder.HasPostgresEnum<StudyPlanTaskStatus>("task_status");
        modelBuilder.HasPostgresEnum<AdjustmentType>();
        modelBuilder.HasPostgresEnum<WaitingListStatus>();
        modelBuilder.HasPostgresEnum<FollowUpEnrollmentStatus>();
        modelBuilder.HasPostgresEnum<TriggerType>();
        modelBuilder.HasPostgresEnum<CallStatus>();
        modelBuilder.HasPostgresEnum<StudentStatus>();
        modelBuilder.HasPostgresEnum<RecommendedAction>();
        modelBuilder.HasPostgresEnum<XpSource>();
        modelBuilder.HasPostgresEnum<PointsTransactionType>();
        modelBuilder.HasPostgresEnum<BenefitType>();
        modelBuilder.HasPostgresEnum<CriteriaType>();
        modelBuilder.HasPostgresEnum<OrderStatus>();
        modelBuilder.HasPostgresEnum<ProductType>();
        modelBuilder.HasPostgresEnum<DiscountType>();
        modelBuilder.HasPostgresEnum<PaymentMethod>();
        modelBuilder.HasPostgresEnum<EnrollmentStatus>();
        modelBuilder.HasPostgresEnum<NotificationType>();
        modelBuilder.HasPostgresEnum<Priority>();
        modelBuilder.HasPostgresEnum<DeliveryChannel>();
        modelBuilder.HasPostgresEnum<DeliveryStatus>();
        modelBuilder.HasPostgresEnum<Platform>();
        modelBuilder.HasPostgresEnum<ConditionType>();
        modelBuilder.HasPostgresEnum<ActionType>();
        modelBuilder.HasPostgresEnum<LogicOperator>();
        modelBuilder.HasPostgresEnum<TargetEntityType>();
        modelBuilder.HasPostgresEnum<ScoreType>();
        modelBuilder.HasPostgresEnum<TimeReferencePoint>();

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var updatedAtProperty = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "UpdatedAtUtc");

            if (updatedAtProperty != null)
            {
                updatedAtProperty.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}