using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Auth.Commands.Login;

/// <summary>
/// Handler for LoginCommand
/// </summary>
public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, ApiResult<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator _jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        this._jwtTokenGenerator = _jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<LoginResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate phone format
        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return ApiResult<LoginResponseDto>.Fail(
                phoneResult.Error.Code,
                phoneResult.Error.Message,
                HttpStatusCodes.BadRequest);

        // 2. Get user by phone
        var user = await _userRepository.GetByPhoneAsync(
            phoneResult.Value, cancellationToken);

        if (user is null)
            return ApiResult<LoginResponseDto>.Fail(
                ErrorCodes.Auth.InvalidCredentials,
                "Invalid phone or password",
                HttpStatusCodes.Unauthorized);

        // 3. Check if user can login (active and verified)
        var canLoginResult = user.CanLogin();
        if (canLoginResult.IsFailure)
            return ApiResult<LoginResponseDto>.Fail(
                canLoginResult.Error.Code,
                canLoginResult.Error.Message,
                HttpStatusCodes.Forbidden);

        // 4. Verify password
        var isPasswordValid = _passwordHasher.VerifyPassword(
            request.Password, user.PasswordHash);

        if (!isPasswordValid)
            return ApiResult<LoginResponseDto>.Fail(
                ErrorCodes.Auth.InvalidCredentials,
                "Invalid phone or password",
                HttpStatusCodes.Unauthorized);

        // 5. Create or update device session
        var existingSessions = await _userRepository.GetActiveSessionsAsync(
            user.Id, cancellationToken);

        var deviceFingerprintHash = HashFingerprint(request.DeviceFingerprint);
        var existingSession = existingSessions
            .FirstOrDefault(s => s.DeviceFingerprintHash == deviceFingerprintHash);

        DeviceSession session;
        if (existingSession is not null)
        {
            // Update existing session
            var refreshResult = existingSession.Refresh(
                _jwtTokenGenerator.GenerateRefreshToken(),
                _jwtTokenGenerator.GetRefreshTokenExpiration());

            if (refreshResult.IsFailure)
                return ApiResult<LoginResponseDto>.Fail(
                    refreshResult.Error.Code,
                    refreshResult.Error.Message,
                    HttpStatusCodes.BadRequest);

            _userRepository.UpdateSession(existingSession);
            session = existingSession;
        }
        else
        {
            // Create new session
            var sessionResult = DeviceSession.Create(
                user.Id,
                deviceFingerprintHash,
                request.Platform,
                _jwtTokenGenerator.GenerateRefreshToken(),
                _jwtTokenGenerator.GetRefreshTokenExpiration());

            if (sessionResult.IsFailure)
                return ApiResult<LoginResponseDto>.Fail(
                    sessionResult.Error.Code,
                    sessionResult.Error.Message,
                    HttpStatusCodes.BadRequest);

            session = sessionResult.Value;
            await _userRepository.AddSessionAsync(session, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Generate JWT tokens
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            user.Id, user.UserType, user.Phone.Value);

        var tokens = new AuthTokensDto(
            accessToken,
            session.RefreshToken,
            session.RefreshTokenExpiresAtUtc);

        // 7. Create response with user info
        var userInfo = new UserInfoDto(
            user.Id.Value,
            user.FullName,
            user.Phone.Value,
            user.UserType.ToString(),
            IsFirstLogin: false);

        var response = new LoginResponseDto(tokens, userInfo);

        return ApiResult<LoginResponseDto>.Ok(response, HttpStatusCodes.Ok);
    }

    private static string HashFingerprint(string fingerprint)
    {
        // Simple hash for demonstration - in production, use a proper hashing algorithm
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(fingerprint);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
