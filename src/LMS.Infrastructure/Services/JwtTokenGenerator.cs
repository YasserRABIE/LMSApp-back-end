using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Infrastructure.Services;

/// <summary>
/// JWT token generator implementation
/// </summary>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured");
        _issuer = configuration["Jwt:Issuer"] ?? "LMS.API";
        _audience = configuration["Jwt:Audience"] ?? "LMS.Client";
        _accessTokenExpiryMinutes = int.Parse(configuration["Jwt:AccessTokenExpiryMinutes"] ?? "5");
        _refreshTokenExpiryDays = int.Parse(configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");
    }

    public string GenerateAccessToken(UserId userId, UserType userType, string phone)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
            new Claim(ClaimTypes.MobilePhone, phone),
            new Claim(ClaimTypes.Role, userType.ToString()),
            new Claim("userType", ((int)userType).ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public DateTime GetRefreshTokenExpiration()
    {
        return DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
    }

    public UserId? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                return UserId.From(userId);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
