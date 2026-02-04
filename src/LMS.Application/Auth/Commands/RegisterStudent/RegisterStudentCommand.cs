using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.RegisterStudent;

/// <summary>
/// Command to register a new student
/// </summary>
public sealed record RegisterStudentCommand(
    string VerificationToken,
    string FullName,
    string Password,
    Guid StudyLevelTrackId,
    string? SchoolName = null,
    string? Governorate = null
) : IRequest<ApiResult<UserInfoDto>>;
