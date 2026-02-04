using FluentValidation;
using MediatR;

namespace LMS.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests using FluentValidation before they reach the handler.
/// Validation errors are automatically converted to ApiResult.ValidationFail responses.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ApiResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators are registered for this request type, skip validation
        if (!_validators.Any())
        {
            return await next();
        }

        // Run all validators for this request
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // If there are validation failures, return a validation error response
        if (failures.Count != 0)
        {
            // Group errors by property name for field-level error messages
            // Use Split('.').Last() to handle nested property names (e.g., "Address.Street" -> "Street")
            var validationErrors = failures
                .GroupBy(f => f.PropertyName.Split('.').Last())
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray());

            // Create validation failure response
            // We need to handle both ApiResult and ApiResult<T>
            var responseType = typeof(TResponse);

            // Check if it's ApiResult<T> (generic)
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResult<>))
            {
                var valueType = responseType.GetGenericArguments()[0];
                var failureMethod = typeof(ApiResult<>)
                    .MakeGenericType(valueType)
                    .GetMethod(nameof(ApiResult.FailureResult), new[] { typeof(string), typeof(string), typeof(Domain.Common.ErrorType) });

                if (failureMethod != null)
                {
                    var errorMessage = "One or more validation errors occurred";
                    return (TResponse)failureMethod.Invoke(null, new object[] {
                        Domain.Common.ErrorCodes.Validation.InvalidInput,
                        errorMessage,
                        Domain.Common.ErrorType.Validation
                    })!;
                }
            }
            // Handle non-generic ApiResult
            else if (responseType == typeof(ApiResult))
            {
                return (TResponse)ApiResult.FailureResult(
                    Domain.Common.ErrorCodes.Validation.InvalidInput,
                    "One or more validation errors occurred",
                    Domain.Common.ErrorType.Validation
                );
            }
        }

        // No validation errors, continue to the handler
        return await next();
    }
}
