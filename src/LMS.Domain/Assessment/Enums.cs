namespace LMS.Domain.Assessment;

/// <summary>
/// Types of questions in an assessment
/// </summary>
public enum QuestionType : byte
{
    /// <summary>
    /// Multiple Choice Question
    /// </summary>
    MCQ = 1,

    /// <summary>
    /// True/False Question
    /// </summary>
    TrueFalse = 2,

    /// <summary>
    /// File Upload (Essay, Code, etc.)
    /// </summary>
    FileUpload = 3
}

/// <summary>
/// Question difficulty levels
/// </summary>
public enum Difficulty : byte
{
    /// <summary>
    /// Easy difficulty
    /// </summary>
    Easy = 1,

    /// <summary>
    /// Medium difficulty
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Hard difficulty
    /// </summary>
    Hard = 3
}

/// <summary>
/// Types of assessments
/// </summary>
public enum AssessmentType : byte
{
    /// <summary>
    /// Short quiz (auto-graded)
    /// </summary>
    Quiz = 1,

    /// <summary>
    /// Assignment (may require manual grading)
    /// </summary>
    Assignment = 2,

    /// <summary>
    /// Final exam for module/course
    /// </summary>
    FinalExam = 3
}

/// <summary>
/// Status of a student's assessment attempt
/// </summary>
public enum AttemptStatus : byte
{
    /// <summary>
    /// Attempt is currently in progress
    /// </summary>
    InProgress = 0,

    /// <summary>
    /// Attempt has been submitted, awaiting grading
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// Attempt has been graded
    /// </summary>
    Graded = 2,

    /// <summary>
    /// Attempt was abandoned by student
    /// </summary>
    Abandoned = 3,

    /// <summary>
    /// Attempt timed out
    /// </summary>
    TimedOut = 4
}
