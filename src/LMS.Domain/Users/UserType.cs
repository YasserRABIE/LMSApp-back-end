namespace LMS.Domain.Users;

/// <summary>
/// Represents the type of user in the system
/// </summary>
public enum UserType
{
    /// <summary>
    /// Student user - can enroll in courses and access content
    /// </summary>
    Student = 1,

    /// <summary>
    /// Teacher user - can create and manage courses
    /// </summary>
    Teacher = 2,

    /// <summary>
    /// Assistant user - provides follow-up support to students
    /// </summary>
    Assistant = 3,

    /// <summary>
    /// Parent user - can monitor student progress
    /// </summary>
    Parent = 4,

    /// <summary>
    /// Admin user - has full system access
    /// </summary>
    Admin = 5
}
