namespace LMS.API.Authorization;

/// <summary>
/// Authorization policy names for role-based access control
/// </summary>
public static class Policies
{
    /// <summary>
    /// Policy that requires the user to be a Student
    /// </summary>
    public const string StudentOnly = "StudentOnly";

    /// <summary>
    /// Policy that requires the user to be a Teacher
    /// </summary>
    public const string TeacherOnly = "TeacherOnly";

    /// <summary>
    /// Policy that requires the user to be an Assistant
    /// </summary>
    public const string AssistantOnly = "AssistantOnly";

    /// <summary>
    /// Policy that requires the user to be a Parent
    /// </summary>
    public const string ParentOnly = "ParentOnly";

    /// <summary>
    /// Policy that requires the user to be an Admin
    /// </summary>
    public const string AdminOnly = "AdminOnly";

    /// <summary>
    /// Policy that requires the user to be either a Teacher or Admin
    /// Used for content management operations (create, update, delete courses/modules/stages/content)
    /// </summary>
    public const string TeachersAndAdmins = "TeachersAndAdmins";

    /// <summary>
    /// Policy that requires the user to be either an Assistant or Admin
    /// Used for follow-up and student support operations
    /// </summary>
    public const string AssistantsAndAdmins = "AssistantsAndAdmins";

    /// <summary>
    /// Policy that requires the user to be either a Student or Parent
    /// Used for accessing learning content and student progress
    /// </summary>
    public const string StudentsAndParents = "StudentsAndParents";

    /// <summary>
    /// Policy that requires any authenticated user
    /// Used for operations available to all logged-in users
    /// </summary>
    public const string AllAuthenticated = "AllAuthenticated";
}
