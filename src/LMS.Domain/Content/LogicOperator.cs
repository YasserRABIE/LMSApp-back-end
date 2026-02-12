namespace LMS.Domain.Content;

/// <summary>
/// Logic operator for combining multiple prerequisite conditions
/// </summary>
public enum LogicOperator
{
    /// <summary>
    /// All conditions must be satisfied
    /// </summary>
    And = 1,

    /// <summary>
    /// At least one condition must be satisfied
    /// </summary>
    Or = 2
}
