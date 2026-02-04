using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using MediatR;

namespace LMS.Application.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for RefreshTokenCommand
/// </summary>
public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, ApiResult<AuthTokensDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<AuthTokensDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get session by refresh token
        var session = await _userRepository.GetSessionByRefreshTokenAsync(
            request.RefreshToken, cancellationToken);

        if (session is null)
            return ApiResult<AuthTokensDto>.Fail(
                ErrorCodes.Auth.InvalidRefreshToken,
                "Invalid refresh token",
                HttpStatusCodes.Unauthorized);

        // 2. Validate refresh token
        if (!session.IsRefreshTokenValid())
            return ApiResult<AuthTokensDto>.Fail(
                ErrorCodes.Auth.RefreshTokenExpired,
                "Refresh token has expired",
                HttpStatusCodes.Unauthorized);

        // 3. Get user
        var user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
            return ApiResult<AuthTokensDto>.Fail(
                ErrorCodes.User.NotFound,
                "User not found",
                HttpStatusCodes.NotFound);

        // 4. Check if user can login
        var canLoginResult = user.CanLogin();
        if (canLoginResult.IsFailure)
            return ApiResult<AuthTokensDto>.Fail(
                canLoginResult.Error.Code,
                canLoginResult.Error.Message,
                HttpStatusCodes.Forbidden);

        // 5. Generate new tokens
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshTokenExpiry = _jwtTokenGenerator.GetRefreshTokenExpiration();

        // 6. Update session
        var refreshResult = session.Refresh(newRefreshToken, newRefreshTokenExpiry);
        if (refreshResult.IsFailure)
            return ApiResult<AuthTokensDto>.Fail(
                refreshResult.Error.Code,
                refreshResult.Error.Message,
                HttpStatusCodes.BadRequest);

        _userRepository.UpdateSession(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Generate new access token
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            user.Id, user.UserType, user.Phone.Value);

        var tokens = new AuthTokensDto(
            accessToken,
            newRefreshToken,
            newRefreshTokenExpiry);

        return ApiResult<AuthTokensDto>.Ok(tokens, HttpStatusCodes.Ok);
    }
}
