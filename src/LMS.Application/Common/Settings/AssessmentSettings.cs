namespace LMS.Application.Common.Settings;

public sealed class AssessmentSettings
{
    public int QuizMaxRetakes { get; init; }
    public int QuizCooldownMinutes { get; init; }
    public int TempAnswerTtlHours { get; init; }
    public int AutoSubmitGracePeriodMinutes { get; init; }
    public decimal LateSubmissionXpMultiplier { get; init; }
    public decimal LateSubmissionPointsMultiplier { get; init; }
    public int DefaultQuizDurationMinutes { get; init; }
    public int DefaultAssignmentDurationDays { get; init; }
    public bool ShuffleQuestionsDefault { get; init; }
    public bool ShuffleOptionsDefault { get; init; }
}
