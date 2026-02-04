using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LMS.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs requests, responses, and execution time
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        // Log request
        _logger.LogInformation(
            "Handling {RequestName} ({RequestId})",
            requestName,
            requestId
        );

        // Start stopwatch
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Execute the request
            var response = await next();

            stopwatch.Stop();

            // Log success
            _logger.LogInformation(
                "Handled {RequestName} ({RequestId}) in {ElapsedMilliseconds}ms",
                requestName,
                requestId,
                stopwatch.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log error
            _logger.LogError(
                ex,
                "Error handling {RequestName} ({RequestId}) after {ElapsedMilliseconds}ms",
                requestName,
                requestId,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}
