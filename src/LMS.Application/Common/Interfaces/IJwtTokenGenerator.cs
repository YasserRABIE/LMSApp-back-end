using LMS.Domain.Users;

namespace LMS.Application.Common.Interfaces;

/// <summary>
/// Service interface for generating JWT tokens
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates an access token for a user
    /// </summary>
    string GenerateAccessToken(UserId userId, UserType userType, string phone);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the refresh token expiration time
    /// </summary>
    DateTime GetRefreshTokenExpiration();

    /// <summary>
    /// Validates a token and extracts the user ID
    /// </summary>
    UserId? ValidateToken(string token);
}
