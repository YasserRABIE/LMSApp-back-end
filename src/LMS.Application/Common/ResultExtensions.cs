using LMS.Domain.Common;
using System.Net;

namespace LMS.Application.Common;

/// <summary>
/// Extension methods for converting Domain Result to ApiResult
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Domain Result to an ApiResult
    /// </summary>
    public static ApiResult ToApiResult(this Result result)
    {
        return result.IsSuccess
            ? ApiResult.SuccessResult()
            : ApiResult.FailureResult(result.Error);
    }

    /// <summary>
    /// Converts a Domain Result<T> to an ApiResult<T>
    /// </summary>
    public static ApiResult<TData> ToApiResult<TData>(this Result<TData> result)
    {
        return result.IsSuccess
            ? ApiResult<TData>.SuccessResult(result.Value)
            : ApiResult<TData>.FailureResult(result.Error);
    }

    /// <summary>
    /// Converts a Domain Result<T> to an ApiResult<T> with a custom status code for success
    /// </summary>
    public static ApiResult<TData> ToApiResult<TData>(this Result<TData> result, HttpStatusCode successStatusCode)
    {
        return result.IsSuccess
            ? ApiResult<TData>.SuccessResult(result.Value, successStatusCode)
            : ApiResult<TData>.FailureResult(result.Error);
    }

    /// <summary>
    /// Converts a Domain Result<T> to an ApiResult<TResponse> with a mapper function
    /// </summary>
    public static ApiResult<TResponse> ToApiResult<TData, TResponse>(
        this Result<TData> result,
        Func<TData, TResponse> mapper)
    {
        return result.IsSuccess
            ? ApiResult<TResponse>.SuccessResult(mapper(result.Value))
            : ApiResult<TResponse>.FailureResult(result.Error);
    }

    /// <summary>
    /// Converts a Domain Result<T> to an ApiResult<TResponse> with an async mapper function
    /// </summary>
    public static async Task<ApiResult<TResponse>> ToApiResultAsync<TData, TResponse>(
        this Result<TData> result,
        Func<TData, Task<TResponse>> mapperAsync)
    {
        return result.IsSuccess
            ? ApiResult<TResponse>.SuccessResult(await mapperAsync(result.Value))
            : ApiResult<TResponse>.FailureResult(result.Error);
    }
}
