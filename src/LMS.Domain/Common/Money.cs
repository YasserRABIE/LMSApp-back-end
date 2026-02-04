namespace LMS.Domain.Common;

/// <summary>
/// Represents money in Egyptian Pounds (EGP)
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency = "EGP")
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a money value in EGP
    /// </summary>
    public static Result<Money> Create(decimal amount, string currency = "EGP")
    {
        if (amount < 0)
        {
            return Result<Money>.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Amount cannot be negative",
                ErrorType.Validation
            );
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return Result<Money>.Failure(
                ErrorCodes.Validation.Required,
                "Currency is required",
                ErrorType.Validation
            );
        }

        // For now, we only support EGP
        if (currency.ToUpperInvariant() != "EGP")
        {
            return Result<Money>.Failure(
                ErrorCodes.Validation.InvalidInput,
                "Only EGP currency is supported",
                ErrorType.Validation
            );
        }

        return Result<Money>.Success(new Money(amount, currency.ToUpperInvariant()));
    }

    /// <summary>
    /// Creates zero money
    /// </summary>
    public static Money Zero() => new(0, "EGP");

    /// <summary>
    /// Adds two money values
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two money values
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies money by a factor
    /// </summary>
    public static Money operator *(Money money, decimal factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>
    /// Divides money by a divisor
    /// </summary>
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException("Cannot divide money by zero");

        return new Money(money.Amount / divisor, money.Currency);
    }

    /// <summary>
    /// Compares if left is greater than right
    /// </summary>
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Compares if left is less than right
    /// </summary>
    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Compares if left is greater than or equal to right
    /// </summary>
    public static bool operator >=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return left.Amount >= right.Amount;
    }

    /// <summary>
    /// Compares if left is less than or equal to right
    /// </summary>
    public static bool operator <=(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");

        return left.Amount <= right.Amount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
