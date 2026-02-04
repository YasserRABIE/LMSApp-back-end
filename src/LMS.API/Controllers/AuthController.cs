using LMS.Application.Auth.Commands.Login;
using LMS.Application.Auth.Commands.Logout;
using LMS.Application.Auth.Commands.LogoutAllDevices;
using LMS.Application.Auth.Commands.RefreshToken;
using LMS.Application.Auth.Commands.RegisterStudent;
using LMS.Application.Auth.Commands.SendOtp;
using LMS.Application.Auth.Commands.VerifyOtp;
using LMS.Application.Auth.DTOs;
using LMS.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Send OTP to phone number for verification
    /// </summary>
    /// <param name="request">Phone number</param>
    /// <returns>Success message if OTP sent</returns>
    [HttpPost("otp/send")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        var command = new SendOtpCommand(request.Phone);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Verify OTP code and get verification token
    /// </summary>
    /// <param name="request">Phone number and OTP code</param>
    /// <returns>Verification token for registration</returns>
    [HttpPost("otp/verify")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<VerificationTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var command = new VerifyOtpCommand(request.Phone, request.Code);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Register a new student account
    /// </summary>
    /// <param name="request">Student registration details</param>
    /// <returns>User information</returns>
    [HttpPost("register/student")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<UserInfoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest request)
    {
        var command = new RegisterStudentCommand(
            request.VerificationToken,
            request.FullName,
            request.Password,
            request.StudyLevelTrackId,
            request.SchoolName,
            request.Governorate);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Login with phone and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Access token, refresh token, and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(
            request.Phone,
            request.Password,
            request.DeviceFingerprint,
            request.Platform);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<AuthTokensDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Logout from current device
    /// </summary>
    /// <param name="request">Session ID to logout</param>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var command = new LogoutCommand(request.SessionId);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Logout from all devices
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAllDevices()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResult.Fail(
                "AUTH.UNAUTHORIZED",
                "User ID not found",
                StatusCodes.Status401Unauthorized));
        }

        var command = new LogoutAllDevicesCommand(userId);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }
}

// Request DTOs
public record SendOtpRequest(string Phone);
public record VerifyOtpRequest(string Phone, string Code);
public record RegisterStudentRequest(
    string VerificationToken,
    string FullName,
    string Password,
    Guid StudyLevelTrackId,
    string? SchoolName = null,
    string? Governorate = null);
public record LoginRequest(string Phone, string Password, string DeviceFingerprint, string Platform);
public record RefreshTokenRequest(string RefreshToken);
public record LogoutRequest(Guid SessionId);
