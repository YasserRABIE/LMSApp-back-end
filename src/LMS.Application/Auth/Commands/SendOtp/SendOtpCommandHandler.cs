using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.SendOtp;

public sealed class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, ApiResult>
{
    private readonly IOtpService _otpService;
    private readonly IUserRepository _userRepository;

    public SendOtpCommandHandler(
        IOtpService otpService,
        IUserRepository userRepository)
    {
        _otpService = otpService;
        _userRepository = userRepository;
    }

    public async Task<ApiResult> Handle(
        SendOtpCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check rate limit
        var isRateLimited = await _otpService.IsRateLimitExceededAsync(
            request.Phone, cancellationToken);

        if (isRateLimited)
            return ApiResult.Fail(
                ErrorCodes.User.OtpRateLimitExceeded,
                ErrorMessages.GetMessage(ErrorCodes.User.OtpRateLimitExceeded),
                HttpStatusCodes.TooManyRequests);

        // 2. Validate phone format
        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return ApiResult.Fail(
                phoneResult.Error.Code,
                ErrorMessages.GetMessage(phoneResult.Error.Code),
                HttpStatusCodes.BadRequest);

        // 3. Check if phone is already registered
        var existingUser = await _userRepository.GetByPhoneAsync(
            phoneResult.Value, cancellationToken);

        if (existingUser is not null)
            return ApiResult.Fail(
                ErrorCodes.User.PhoneAlreadyExists,
                ErrorMessages.GetMessage(ErrorCodes.User.PhoneAlreadyExists),
                HttpStatusCodes.Conflict);

        // 4. Generate OTP
        var otpCode = await _otpService.GenerateOtpAsync(
            request.Phone, cancellationToken);

        // 5. Send OTP via SMS
        var sendResult = await _otpService.SendOtpSmsAsync(
            request.Phone, otpCode, cancellationToken);

        if (sendResult.IsFailure)
            return ApiResult.Fail(
                sendResult.Error.Code,
                ErrorMessages.GetMessage(sendResult.Error.Code),
                HttpStatusCodes.InternalServerError);

        return ApiResult.Ok(SuccessMessages.OtpSent);
    }
}
