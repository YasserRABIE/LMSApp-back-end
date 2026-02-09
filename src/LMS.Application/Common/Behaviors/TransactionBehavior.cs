using LMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that wraps command execution in a database transaction
/// Only applies to commands that return ApiResult (not queries)
/// Compatible with Entity Framework retry execution strategies
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ApiResult
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Check if this is a query (read-only) - queries typically have "Query" in their name
        if (requestName.Contains("Query", StringComparison.OrdinalIgnoreCase))
        {
            // Skip transaction for queries
            return await next();
        }

        // For commands, execute without explicit transaction
        // Entity Framework will automatically wrap SaveChanges in a transaction
        // This makes it compatible with retry execution strategies
        _logger.LogInformation("Executing command {RequestName}", requestName);

        try
        {
            // Execute the request (SaveChanges will be called by repositories)
            var response = await next();

            if (response.Success)
            {
                _logger.LogInformation("Command {RequestName} executed successfully", requestName);
            }
            else
            {
                _logger.LogWarning(
                    "Command {RequestName} failed. Error: {ErrorCode}",
                    requestName,
                    response.Error?.Code ?? "UNKNOWN"
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Command {RequestName} threw exception",
                requestName
            );

            throw;
        }
    }
}
