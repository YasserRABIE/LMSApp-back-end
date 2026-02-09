using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Auth.Commands.LogoutAllDevices;

public sealed class LogoutAllDevicesCommandHandler : IRequestHandler<LogoutAllDevicesCommand, ApiResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutAllDevicesCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(
        LogoutAllDevicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.UserId);

        // 1. Check user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return ApiResult.Fail(
                ErrorCodes.User.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.User.NotFound),
                HttpStatusCodes.NotFound);

        // 2. Revoke all sessions
        await _userRepository.RevokeAllSessionsAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.LogoutAllDevicesSuccess);
    }
}
