using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.RegisterStudent;

/// <summary>
/// Command to register a new student
/// Phone verification removed - accepts phone directly
/// </summary>
public sealed record RegisterStudentCommand(
    string Phone,
    string FirstName,
    string SecondName,
    string LastName,
    string Password,
    Guid StudyLevelTrackId,
    string DeviceFingerprint,
    string Platform,
    string? SchoolName = null,
    string? Governorate = null
) : IRequest<ApiResult<LoginResponseDto>>;
