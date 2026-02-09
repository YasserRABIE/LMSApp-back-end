namespace LMS.Domain.StudyPlan;

/// <summary>
/// Status of a study plan
/// </summary>
public enum PlanStatus : byte
{
    /// <summary>
    /// Plan is currently active
    /// </summary>
    Active = 0,

    /// <summary>
    /// Plan has been completed
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Plan was abandoned by student
    /// </summary>
    Abandoned = 2
}

/// <summary>
/// Type of day in a study plan
/// </summary>
public enum DayType : byte
{
    /// <summary>
    /// Regular study day with new content
    /// </summary>
    Study = 1,

    /// <summary>
    /// Review day for previously covered material
    /// </summary>
    Review = 2,

    /// <summary>
    /// Rest day (no tasks assigned)
    /// </summary>
    Rest = 3,

    /// <summary>
    /// Follow-up session day
    /// </summary>
    FollowUp = 4
}

/// <summary>
/// Status of a plan day
/// </summary>
public enum DayStatus : byte
{
    /// <summary>
    /// Day is in the future, not yet started
    /// </summary>
    Upcoming = 0,

    /// <summary>
    /// Day has been completed
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Day was missed
    /// </summary>
    Missed = 2,

    /// <summary>
    /// Day was rescheduled to another date
    /// </summary>
    Rescheduled = 3
}

/// <summary>
/// Type of task in a study plan
/// </summary>
public enum TaskType : byte
{
    /// <summary>
    /// Watch a lecture/video
    /// </summary>
    Lecture = 1,

    /// <summary>
    /// Complete a quiz
    /// </summary>
    Quiz = 2,

    /// <summary>
    /// Complete an assignment
    /// </summary>
    Assignment = 3,

    /// <summary>
    /// Review previously covered content
    /// </summary>
    Review = 4,

    /// <summary>
    /// Take a review quiz on previous material
    /// </summary>
    ReviewQuiz = 5
}

/// <summary>
/// Status of a task in a study plan
/// </summary>
public enum TaskStatus : byte
{
    /// <summary>
    /// Task is pending, not yet started
    /// </summary>
    Todo = 0,

    /// <summary>
    /// Task is currently in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task has been completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Task was missed (deadline passed)
    /// </summary>
    Missed = 3,

    /// <summary>
    /// Task was skipped by student
    /// </summary>
    Skipped = 4
}
