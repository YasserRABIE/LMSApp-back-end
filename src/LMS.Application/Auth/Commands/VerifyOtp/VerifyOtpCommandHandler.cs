using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.VerifyOtp;

/// <summary>
/// Handler for VerifyOtpCommand
/// </summary>
public sealed class VerifyOtpCommandHandler
    : IRequestHandler<VerifyOtpCommand, ApiResult<VerificationTokenDto>>
{
    private readonly IOtpService _otpService;

    public VerifyOtpCommandHandler(IOtpService otpService)
    {
        _otpService = otpService;
    }

    public async Task<ApiResult<VerificationTokenDto>> Handle(
        VerifyOtpCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verify OTP
        var isValid = await _otpService.VerifyOtpAsync(
            request.Phone, request.Code, cancellationToken);

        if (!isValid)
            return ApiResult<VerificationTokenDto>.Fail(
                ErrorCodes.User.InvalidOtp,
                "Invalid or expired OTP code",
                HttpStatusCodes.BadRequest);

        // 2. Generate verification token (valid for 15 minutes)
        var verificationToken = await _otpService.GenerateVerificationTokenAsync(
            request.Phone, cancellationToken);

        // 3. Return verification token
        var dto = new VerificationTokenDto(
            verificationToken,
            DateTime.UtcNow.AddMinutes(15));

        return ApiResult<VerificationTokenDto>.Ok(dto, HttpStatusCodes.Ok);
    }
}
