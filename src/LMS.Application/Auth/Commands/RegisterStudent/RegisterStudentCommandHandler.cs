using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Auth.Commands.RegisterStudent;

/// <summary>
/// Handler for RegisterStudentCommand
/// </summary>
public sealed class RegisterStudentCommandHandler
    : IRequestHandler<RegisterStudentCommand, ApiResult<UserInfoDto>>
{
    private readonly IOtpService _otpService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterStudentCommandHandler(
        IOtpService otpService,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _otpService = otpService;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<UserInfoDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate verification token and get phone
        var phoneNumber = await _otpService.ValidateVerificationTokenAsync(
            request.VerificationToken, cancellationToken);

        if (phoneNumber is null)
            return ApiResult<UserInfoDto>.Fail(
                ErrorCodes.Auth.InvalidVerificationToken,
                "Invalid or expired verification token",
                HttpStatusCodes.BadRequest);

        // 2. Validate phone format
        var phoneResult = Phone.Create(phoneNumber);
        if (phoneResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                phoneResult.Error.Code,
                phoneResult.Error.Message,
                HttpStatusCodes.BadRequest);

        // 3. Check if phone already registered
        var existingUser = await _userRepository.GetByPhoneAsync(
            phoneResult.Value, cancellationToken);

        if (existingUser is not null)
            return ApiResult<UserInfoDto>.Fail(
                ErrorCodes.User.PhoneAlreadyExists,
                "Phone number already registered",
                HttpStatusCodes.Conflict);

        // 4. Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 5. Create user entity
        var userResult = User.Create(
            phoneResult.Value,
            passwordHash,
            request.FullName,
            UserType.Student,
            isPhoneVerified: true);

        if (userResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                userResult.Error.Code,
                userResult.Error.Message,
                HttpStatusCodes.BadRequest);

        var user = userResult.Value;

        // 6. Create student profile
        var profileResult = StudentProfile.Create(
            user.Id,
            request.StudyLevelTrackId,
            request.SchoolName,
            request.Governorate);

        if (profileResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                profileResult.Error.Code,
                profileResult.Error.Message,
                HttpStatusCodes.BadRequest);

        // 7. Save user and profile
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.AddStudentProfileAsync(profileResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Map to DTO and return
        var dto = new UserInfoDto(
            user.Id.Value,
            user.FullName,
            user.Phone.Value,
            user.UserType.ToString(),
            IsFirstLogin: false);

        return ApiResult<UserInfoDto>.Ok(dto, HttpStatusCodes.Created);
    }
}
