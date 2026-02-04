using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LMS.Infrastructure.Persistence.Converters;

/// <summary>
/// Centralized value converters for strongly-typed IDs to enable EF Core mapping.
/// Provides consistent conversion logic across all entity configurations.
/// </summary>
public static class StronglyTypedIdConverters
{
    /// <summary>
    /// Converter for UserId strongly-typed ID
    /// </summary>
    public static readonly ValueConverter<UserId, Guid> UserIdConverter = new(
        id => id.Value,
        value => UserId.From(value));
}
