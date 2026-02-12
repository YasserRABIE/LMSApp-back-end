using LMS.Application.Common;
using LMS.Application.Features.Auth.DTOs;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Command to verify an OTP code for a phone number
/// </summary>
public sealed record VerifyOtpCommand(
    string Phone,
    string Code
) : IRequest<ApiResult<VerificationTokenDto>>;
