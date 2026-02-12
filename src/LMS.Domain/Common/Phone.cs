using System.Text.RegularExpressions;

namespace LMS.Domain.Common;

/// <summary>
/// Represents an Egyptian phone number value object
/// Format: 01XXXXXXXXX (11 digits starting with 01)
/// </summary>
public sealed class Phone : ValueObject
{
    /// <summary>
    /// Egyptian phone number regex pattern
    /// Matches: 01XXXXXXXXX (11 digits, starts with 01)
    /// Valid prefixes: 010, 011, 012, 015 (major Egyptian operators)
    /// </summary>
    private static readonly Regex PhoneRegex = new(
        @"^01[0125]\d{8}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; private set; }

    // Parameterless constructor for EF Core
    private Phone()
    {
        Value = string.Empty;
    }

    private Phone(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a phone number from a string
    /// </summary>
    public static Result<Phone> Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return Result<Phone>.Failure(
                Error.Validation(ErrorCodes.User.InvalidPhone)
            );
        }

        // Remove any whitespace or special characters
        string cleanedPhone = Regex.Replace(phone, @"[\s\-\(\)]", string.Empty);

        // Handle phone numbers with country code (+20)
        if (cleanedPhone.StartsWith("+20"))
        {
            cleanedPhone = "0" + cleanedPhone.Substring(3);
        }
        else if (cleanedPhone.StartsWith("20") && cleanedPhone.Length == 12)
        {
            cleanedPhone = "0" + cleanedPhone.Substring(2);
        }

        if (!PhoneRegex.IsMatch(cleanedPhone))
        {
            return Result<Phone>.Failure(
                Error.Validation(ErrorCodes.User.InvalidPhone)
            );
        }

        return Result<Phone>.Success(new Phone(cleanedPhone));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    /// <summary>
    /// Implicit conversion from Phone to string
    /// </summary>
    public static implicit operator string(Phone phone) => phone.Value;
}
