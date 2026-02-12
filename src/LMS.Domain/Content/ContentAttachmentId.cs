namespace LMS.Domain.Content;

/// <summary>
/// Strongly-typed identifier for ContentAttachment entity
/// </summary>
public sealed record ContentAttachmentId(Guid Value)
{
    /// <summary>
    /// Creates a new unique ContentAttachmentId
    /// </summary>
    public static ContentAttachmentId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ContentAttachmentId from an existing Guid
    /// </summary>
    public static ContentAttachmentId From(Guid value) => new(value);

    /// <summary>
    /// Implicit conversion from ContentAttachmentId to Guid
    /// </summary>
    public static implicit operator Guid(ContentAttachmentId id) => id.Value;

    public override string ToString() => Value.ToString();
}
