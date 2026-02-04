namespace LMS.Domain.Common;

/// <summary>
/// Represents the result of an operation that doesn't return a value
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error details (null if successful)
    /// </summary>
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a success result
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a failure result
    /// </summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a failure result with code and message
    /// </summary>
    public static Result Failure(string code, string message, ErrorType type = ErrorType.Failure)
        => new(false, Error.Create(code, message, type));

    /// <summary>
    /// Implicit conversion from Result to bool
    /// </summary>
    public static implicit operator bool(Result result) => result.IsSuccess;
}

/// <summary>
/// Represents the result of an operation that returns a value
/// </summary>
/// <typeparam name="TValue">The type of the value returned</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// The value returned by the operation (throws if accessed on failure)
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result");

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Creates a success result with a value
    /// </summary>
    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// Creates a failure result
    /// </summary>
    public new static Result<TValue> Failure(Error error) => new(default, false, error);

    /// <summary>
    /// Creates a failure result with code and message
    /// </summary>
    public new static Result<TValue> Failure(string code, string message, ErrorType type = ErrorType.Failure)
        => new(default, false, Error.Create(code, message, type));

    /// <summary>
    /// Implicit conversion from value to Result<TValue>
    /// </summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);
}
