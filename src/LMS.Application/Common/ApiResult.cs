using LMS.Domain.Common;
using System.Net;

namespace LMS.Application.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResult
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Error details (null if successful)
    /// </summary>
    public ApiError? Error { get; init; }

    /// <summary>
    /// Success message (null if failed)
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; init; }

    protected ApiResult(bool success, ApiError? error, string? message, int statusCode)
    {
        Success = success;
        Error = error;
        Message = message;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResult SuccessResult() =>
        new(true, null, null, (int)HttpStatusCode.OK);

    /// <summary>
    /// Creates a successful response with custom status code
    /// </summary>
    public static ApiResult Ok(int statusCode = HttpStatusCodes.Ok) =>
        new(true, null, null, statusCode);

    /// <summary>
    /// Creates a successful response with a message
    /// </summary>
    public static ApiResult Ok(string message, int statusCode = HttpStatusCodes.Ok) =>
        new(true, null, message, statusCode);

    /// <summary>
    /// Creates a failure response from an error
    /// </summary>
    public static ApiResult FailureResult(Error error) =>
        new(false, ApiError.FromError(error), null, GetStatusCode(error.Type));

    /// <summary>
    /// Creates a failure response from error details
    /// </summary>
    public static ApiResult FailureResult(string code, string message, ErrorType type = ErrorType.Failure) =>
        new(false, new ApiError(code, message), null, GetStatusCode(type));

    /// <summary>
    /// Creates a failure response with explicit status code
    /// </summary>
    public static ApiResult Fail(string code, string message, int statusCode) =>
        new(false, new ApiError(code, message), null, statusCode);

    /// <summary>
    /// Creates a failure response with validation errors
    /// </summary>
    public static ApiResult Fail(string code, string message, int statusCode, Dictionary<string, string[]> validationErrors) =>
        new(false, new ApiError(code, message, validationErrors), null, statusCode);

    /// <summary>
    /// Maps ErrorType to HTTP status code
    /// </summary>
    protected static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.None => (int)HttpStatusCode.OK,
        ErrorType.Validation => (int)HttpStatusCode.BadRequest,
        ErrorType.NotFound => (int)HttpStatusCode.NotFound,
        ErrorType.Conflict => (int)HttpStatusCode.Conflict,
        ErrorType.Unauthorized => (int)HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => (int)HttpStatusCode.Forbidden,
        ErrorType.Failure => (int)HttpStatusCode.BadRequest,
        _ => (int)HttpStatusCode.InternalServerError
    };
}

/// <summary>
/// Standard API response wrapper with data
/// </summary>
/// <typeparam name="TData">The type of data returned</typeparam>
public class ApiResult<TData> : ApiResult
{
    /// <summary>
    /// The data returned by the operation (null if failed)
    /// </summary>
    public TData? Data { get; init; }

    private ApiResult(TData? data, bool success, ApiError? error, string? message, int statusCode)
        : base(success, error, message, statusCode)
    {
        Data = data;
    }

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ApiResult<TData> SuccessResult(TData data) =>
        new(data, true, null, null, (int)HttpStatusCode.OK);

    /// <summary>
    /// Creates a successful response with data and custom status code
    /// </summary>
    public static ApiResult<TData> SuccessResult(TData data, HttpStatusCode statusCode) =>
        new(data, true, null, null, (int)statusCode);

    /// <summary>
    /// Creates a successful response with data and optional message
    /// </summary>
    public static ApiResult<TData> Ok(TData data, int statusCode = HttpStatusCodes.Ok) =>
        new(data, true, null, null, statusCode);

    /// <summary>
    /// Creates a successful response with data and success message
    /// </summary>
    public static ApiResult<TData> Ok(TData data, string message, int statusCode = HttpStatusCodes.Ok) =>
        new(data, true, null, message, statusCode);

    /// <summary>
    /// Creates a failure response from an error
    /// </summary>
    public new static ApiResult<TData> FailureResult(Error error) =>
        new(default, false, ApiError.FromError(error), null, GetStatusCode(error.Type));

    /// <summary>
    /// Creates a failure response from error details
    /// </summary>
    public new static ApiResult<TData> FailureResult(string code, string message, ErrorType type = ErrorType.Failure) =>
        new(default, false, new ApiError(code, message), null, GetStatusCode(type));

    /// <summary>
    /// Creates a failure response with explicit status code
    /// </summary>
    public new static ApiResult<TData> Fail(string code, string message, int statusCode) =>
        new(default, false, new ApiError(code, message), null, statusCode);

    /// <summary>
    /// Creates a failure response with validation errors
    /// </summary>
    public new static ApiResult<TData> Fail(string code, string message, int statusCode, Dictionary<string, string[]> validationErrors) =>
        new(default, false, new ApiError(code, message, validationErrors), null, statusCode);
}

/// <summary>
/// API error representation
/// </summary>
public class ApiError
{
    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Validation errors (null if not a validation error)
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public ApiError(string code, string message, Dictionary<string, string[]>? validationErrors = null)
    {
        Code = code;
        Message = message;
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Creates an ApiError from a domain Error with localized message
    /// </summary>
    public static ApiError FromError(Error error) =>
        new(error.Code, ErrorMessages.GetMessage(error.Code));
}
