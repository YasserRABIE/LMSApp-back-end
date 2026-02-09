using LMS.Domain.Common;

namespace LMS.Domain.Gamification;

public sealed class Level : Entity<Guid>
{
    public short Number { get; private set; }
    public string Name { get; private set; }
    public string? NameEn { get; private set; }
    public int RequiredXp { get; private set; }
    public string BadgeImageUrl { get; private set; }
    public string ShortMessage { get; private set; }
    public string LongMessage { get; private set; }

    private Level() : base() { Name = string.Empty; BadgeImageUrl = string.Empty; ShortMessage = string.Empty; LongMessage = string.Empty; }

    public static Result<Level> Create(short number, string name, int requiredXp, string badgeImageUrl, string shortMessage, string longMessage, string? nameEn = null)
    {
        if (number < 1) return Result<Level>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        if (string.IsNullOrWhiteSpace(name)) return Result<Level>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<Level>.Success(new Level { Id = Guid.NewGuid(), Number = number, Name = name, RequiredXp = requiredXp, BadgeImageUrl = badgeImageUrl, ShortMessage = shortMessage, LongMessage = longMessage, NameEn = nameEn });
    }
}
