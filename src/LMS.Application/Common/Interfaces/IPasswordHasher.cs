namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Service interface for password hashing operations
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password using BCrypt
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <returns>The hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <param name="hash">The password hash</param>
    /// <returns>True if the password matches the hash, false otherwise</returns>
    bool VerifyPassword(string password, string hash);
}
