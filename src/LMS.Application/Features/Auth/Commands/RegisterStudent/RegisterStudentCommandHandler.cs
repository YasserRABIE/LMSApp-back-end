using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Auth.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.RegisterStudent;

public sealed class RegisterStudentCommandHandler
    : IRequestHandler<RegisterStudentCommand, ApiResult<LoginResponseDto>>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterStudentCommandHandler(
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ApiResult<LoginResponseDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate phone format (phone verification removed)
        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return ApiResult<LoginResponseDto>.Fail(
                phoneResult.Error.Code,
                ErrorMessages.GetMessage(phoneResult.Error.Code),
                HttpStatusCodes.BadRequest);

        // 2. Check if phone already registered
        var existingUser = await _userRepository.GetByPhoneAsync(
            phoneResult.Value, cancellationToken);

        if (existingUser is not null)
            return ApiResult<LoginResponseDto>.Fail(
                ErrorCodes.User.PhoneAlreadyExists,
                ErrorMessages.GetMessage(ErrorCodes.User.PhoneAlreadyExists),
                HttpStatusCodes.Conflict);

        // 3. Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 4. Create user entity (phone verified in development since OTP is bypassed)
        var userResult = User.Create(
            phoneResult.Value,
            passwordHash,
            request.FirstName,
            request.SecondName,
            request.LastName,
            UserType.Student,
            isPhoneVerified: true);

        if (userResult.IsFailure)
            return ApiResult<LoginResponseDto>.Fail(
                userResult.Error.Code,
                ErrorMessages.GetMessage(userResult.Error.Code),
                HttpStatusCodes.BadRequest);

        var user = userResult.Value;

        // 5. Create student profile
        var profileResult = StudentProfile.Create(
            user.Id,
            request.StudyLevelTrackId,
            request.SchoolName,
            request.Governorate);

        if (profileResult.IsFailure)
            return ApiResult<LoginResponseDto>.Fail(
                profileResult.Error.Code,
                ErrorMessages.GetMessage(profileResult.Error.Code),
                HttpStatusCodes.BadRequest);

        // 6. Save user and profile
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.AddStudentProfileAsync(profileResult.Value, cancellationToken);

        // 7. Create device session
        var deviceFingerprintHash = HashFingerprint(request.DeviceFingerprint);
        var sessionResult = DeviceSession.Create(
            user.Id,
            deviceFingerprintHash,
            request.Platform,
            _jwtTokenGenerator.GenerateRefreshToken(),
            _jwtTokenGenerator.GetRefreshTokenExpiration());

        if (sessionResult.IsFailure)
            return ApiResult<LoginResponseDto>.Fail(
                sessionResult.Error.Code,
                ErrorMessages.GetMessage(sessionResult.Error.Code),
                HttpStatusCodes.BadRequest);

        var session = sessionResult.Value;
        await _userRepository.AddSessionAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Generate JWT tokens
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            user.Id, user.UserType, user.Phone.Value);

        var tokens = new AuthTokensDto(
            accessToken,
            session.RefreshToken,
            session.RefreshTokenExpiresAtUtc);

        // 9. Create response with user info
        var userInfo = new UserInfoDto(
            user.Id.Value,
            user.GetFullName(),
            user.Phone.Value,
            user.UserType.ToString(),
            IsFirstLogin: true);

        var response = new LoginResponseDto(tokens, userInfo);

        return ApiResult<LoginResponseDto>.Ok(response, SuccessMessages.RegistrationSuccess, HttpStatusCodes.Created);
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
