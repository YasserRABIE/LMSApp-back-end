using LMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that wraps command execution in a database transaction
/// Only applies to commands that return ApiResult (not queries)
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

        _logger.LogInformation("Starting transaction for {RequestName}", requestName);

        try
        {
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Execute the request
            var response = await next();

            // If successful, commit transaction
            if (response.Success)
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction committed for {RequestName}", requestName);
            }
            else
            {
                // If failed, rollback transaction
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning(
                    "Transaction rolled back for {RequestName}. Error: {ErrorCode}",
                    requestName,
                    response.Error?.Code ?? "UNKNOWN"
                );
            }

            return response;
        }
        catch (Exception ex)
        {
            // Rollback on exception
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _logger.LogError(
                ex,
                "Transaction rolled back for {RequestName} due to exception",
                requestName
            );

            throw;
        }
    }
}
