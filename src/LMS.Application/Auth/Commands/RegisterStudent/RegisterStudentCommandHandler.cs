using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Auth.Commands.RegisterStudent;

public sealed class RegisterStudentCommandHandler
    : IRequestHandler<RegisterStudentCommand, ApiResult<UserInfoDto>>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterStudentCommandHandler(
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<UserInfoDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate phone format (phone verification removed)
        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                phoneResult.Error.Code,
                ErrorMessages.GetMessage(phoneResult.Error.Code),
                HttpStatusCodes.BadRequest);

        // 2. Check if phone already registered
        var existingUser = await _userRepository.GetByPhoneAsync(
            phoneResult.Value, cancellationToken);

        if (existingUser is not null)
            return ApiResult<UserInfoDto>.Fail(
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
            return ApiResult<UserInfoDto>.Fail(
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
            return ApiResult<UserInfoDto>.Fail(
                profileResult.Error.Code,
                ErrorMessages.GetMessage(profileResult.Error.Code),
                HttpStatusCodes.BadRequest);

        // 6. Save user and profile
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.AddStudentProfileAsync(profileResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Map to DTO and return with success message
        var dto = new UserInfoDto(
            user.Id.Value,
            user.GetFullName(),
            user.Phone.Value,
            user.UserType.ToString(),
            IsFirstLogin: false);

        return ApiResult<UserInfoDto>.Ok(dto, SuccessMessages.RegistrationSuccess, HttpStatusCodes.Created);
    }
}
