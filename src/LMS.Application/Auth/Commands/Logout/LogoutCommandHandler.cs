using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get session
        var session = await _userRepository.GetSessionByIdAsync(
            request.SessionId, cancellationToken);

        if (session is null)
            return ApiResult.Fail(
                ErrorCodes.Auth.SessionNotFound,
                ErrorMessages.GetMessage(ErrorCodes.Auth.SessionNotFound),
                HttpStatusCodes.NotFound);

        // 2. Revoke session
        var revokeResult = session.Revoke();
        if (revokeResult.IsFailure)
            return ApiResult.Fail(
                revokeResult.Error.Code,
                ErrorMessages.GetMessage(revokeResult.Error.Code),
                HttpStatusCodes.BadRequest);

        _userRepository.UpdateSession(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.LogoutSuccess);
    }
}
