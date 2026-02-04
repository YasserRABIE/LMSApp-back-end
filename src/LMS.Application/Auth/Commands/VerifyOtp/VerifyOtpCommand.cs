using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.VerifyOtp;

/// <summary>
/// Command to verify an OTP code for a phone number
/// </summary>
public sealed record VerifyOtpCommand(
    string Phone,
    string Code
) : IRequest<ApiResult<VerificationTokenDto>>;
