using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.SendOtp;

/// <summary>
/// Command to send an OTP to a phone number
/// </summary>
public sealed record SendOtpCommand(
    string Phone
) : IRequest<ApiResult>;
