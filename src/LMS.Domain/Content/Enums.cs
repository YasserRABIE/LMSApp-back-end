namespace LMS.Domain.Content;

/// <summary>
/// Content visibility status
/// </summary>
public enum Visibility : byte
{
    /// <summary>
    /// Hidden from all users
    /// </summary>
    Hidden = 0,

    /// <summary>
    /// Draft - visible only to teacher
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Published - visible to students
    /// </summary>
    Published = 2
}

/// <summary>
/// Types of content items
/// </summary>
public enum ContentType : byte
{
    /// <summary>
    /// Video lecture
    /// </summary>
    Video = 1,

    /// <summary>
    /// Downloadable file (PDF, document, etc.)
    /// </summary>
    File = 2,

    /// <summary>
    /// Quiz assessment
    /// </summary>
    Quiz = 3,

    /// <summary>
    /// Assignment to submit
    /// </summary>
    Assignment = 4,

    /// <summary>
    /// Live workshop session
    /// </summary>
    Workshop = 5
}

/// <summary>
/// Video processing status
/// </summary>
public enum VideoStatus : byte
{
    /// <summary>
    /// Video is being processed by provider
    /// </summary>
    Processing = 0,

    /// <summary>
    /// Video is ready for playback
    /// </summary>
    Ready = 1,

    /// <summary>
    /// Video processing failed
    /// </summary>
    Failed = 2
}

/// <summary>
/// Types of content prerequisites
/// </summary>
public enum PrerequisiteType : byte
{
    /// <summary>
    /// Must complete specific content item
    /// </summary>
    CompleteContent = 1,

    /// <summary>
    /// Must complete entire stage
    /// </summary>
    CompleteStage = 2,

    /// <summary>
    /// Must complete entire module
    /// </summary>
    CompleteModule = 3,

    /// <summary>
    /// Must pass assessment with minimum score
    /// </summary>
    PassAssessment = 4,

    /// <summary>
    /// Must receive follow-up approval
    /// </summary>
    FollowUpApproval = 5,

    /// <summary>
    /// Must purchase content/module/course
    /// </summary>
    Purchase = 6,

    /// <summary>
    /// Must wait specific number of days
    /// </summary>
    TimeDelay = 7
}

/// <summary>
/// Student progress status for content
/// </summary>
public enum ProgressStatus : byte
{
    /// <summary>
    /// Student has not started this content
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Student is currently working on this content
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Student has completed this content
    /// </summary>
    Completed = 2
}

/// <summary>
/// Logical operator for prerequisite groups
/// </summary>
public enum GroupOperator : byte
{
    /// <summary>
    /// All prerequisites in group must be satisfied
    /// </summary>
    And = 1,

    /// <summary>
    /// At least one prerequisite in group must be satisfied
    /// </summary>
    Or = 2
}
