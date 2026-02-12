using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class ContentAttachment : Entity<ContentAttachmentId>
{
    public ContentItemId ContentItemId { get; private set; }
    public string FileName { get; private set; }
    public string FileUrl { get; private set; }
    public long FileSize { get; private set; }
    public string MimeType { get; private set; }
    public int DisplayOrder { get; private set; }

    private ContentAttachment() : base()
    {
        ContentItemId = null!;
        FileName = string.Empty;
        FileUrl = string.Empty;
        MimeType = string.Empty;
    }

    private ContentAttachment(
        ContentAttachmentId id,
        ContentItemId contentItemId,
        string fileName,
        string fileUrl,
        long fileSize,
        string mimeType,
        int displayOrder) : base(id)
    {
        ContentItemId = contentItemId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileSize = fileSize;
        MimeType = mimeType;
        DisplayOrder = displayOrder;
    }

    public static Result<ContentAttachment> Create(
        ContentItemId contentItemId,
        string fileName,
        string fileUrl,
        long fileSize,
        string mimeType,
        int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return Result<ContentAttachment>.Failure(Error.Validation(ErrorCodes.Content.FileNameRequired));

        if (string.IsNullOrWhiteSpace(fileUrl))
            return Result<ContentAttachment>.Failure(Error.Validation(ErrorCodes.Content.FileUrlRequired));

        if (fileSize <= 0)
            return Result<ContentAttachment>.Failure(Error.Validation(ErrorCodes.Content.InvalidFileSize));

        if (string.IsNullOrWhiteSpace(mimeType))
            return Result<ContentAttachment>.Failure(Error.Validation(ErrorCodes.Content.MimeTypeRequired));

        var attachment = new ContentAttachment(
            ContentAttachmentId.New(),
            contentItemId,
            fileName.Trim(),
            fileUrl.Trim(),
            fileSize,
            mimeType.Trim(),
            displayOrder);

        return Result<ContentAttachment>.Success(attachment);
    }

    public void UpdateDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
}
