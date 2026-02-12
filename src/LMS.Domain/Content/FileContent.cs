using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class FileContent : Entity<Guid>
{
    public ContentItemId ContentItemId { get; private set; }
    public string StorageProvider { get; private set; }
    public string StoragePath { get; private set; }
    public string? PublicUrl { get; private set; }
    public string OriginalFileName { get; private set; }
    public string FileExtension { get; private set; }
    public string MimeType { get; private set; }
    public long FileSize { get; private set; }
    public bool AllowDownload { get; private set; }

    private FileContent() : base()
    {
        ContentItemId = null!;
        StorageProvider = string.Empty;
        StoragePath = string.Empty;
        OriginalFileName = string.Empty;
        FileExtension = string.Empty;
        MimeType = string.Empty;
    }

    private FileContent(
        Guid id,
        ContentItemId contentItemId,
        string storageProvider,
        string storagePath,
        string originalFileName,
        string fileExtension,
        string mimeType,
        long fileSize,
        string? publicUrl,
        bool allowDownload) : base(id)
    {
        ContentItemId = contentItemId;
        StorageProvider = storageProvider;
        StoragePath = storagePath;
        OriginalFileName = originalFileName;
        FileExtension = fileExtension;
        MimeType = mimeType;
        FileSize = fileSize;
        PublicUrl = publicUrl;
        AllowDownload = allowDownload;
    }

    public static Result<FileContent> Create(
        ContentItemId contentItemId,
        string storageProvider,
        string storagePath,
        string originalFileName,
        string fileExtension,
        string mimeType,
        long fileSize,
        string? publicUrl = null,
        bool allowDownload = false)
    {
        if (string.IsNullOrWhiteSpace(storageProvider))
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.StorageProviderRequired));

        if (storageProvider.Length > 20)
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.StorageProviderTooLong));

        if (string.IsNullOrWhiteSpace(storagePath))
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.StoragePathRequired));

        if (storagePath.Length > 1000)
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.StoragePathTooLong));

        if (string.IsNullOrWhiteSpace(originalFileName))
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.FileNameRequired));

        if (originalFileName.Length > 300)
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.FileNameTooLong));

        if (string.IsNullOrWhiteSpace(fileExtension))
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.FileExtensionRequired));

        if (fileExtension.Length > 20)
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.FileExtensionTooLong));

        if (string.IsNullOrWhiteSpace(mimeType))
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.MimeTypeRequired));

        if (mimeType.Length > 100)
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.MimeTypeTooLong));

        if (fileSize <= 0)
            return Result<FileContent>.Failure(Error.Validation(ErrorCodes.Content.InvalidFileSize));

        var fileContent = new FileContent(
            Guid.NewGuid(),
            contentItemId,
            storageProvider.Trim(),
            storagePath.Trim(),
            originalFileName.Trim(),
            fileExtension.Trim(),
            mimeType.Trim(),
            fileSize,
            publicUrl?.Trim(),
            allowDownload);

        return Result<FileContent>.Success(fileContent);
    }

    public Result UpdateMetadata(string originalFileName, string fileExtension, string mimeType, long fileSize)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
            return Result.Failure(Error.Validation(ErrorCodes.Content.FileNameRequired));

        if (originalFileName.Length > 300)
            return Result.Failure(Error.Validation(ErrorCodes.Content.FileNameTooLong));

        if (string.IsNullOrWhiteSpace(fileExtension))
            return Result.Failure(Error.Validation(ErrorCodes.Content.FileExtensionRequired));

        if (string.IsNullOrWhiteSpace(mimeType))
            return Result.Failure(Error.Validation(ErrorCodes.Content.MimeTypeRequired));

        if (fileSize <= 0)
            return Result.Failure(Error.Validation(ErrorCodes.Content.InvalidFileSize));

        OriginalFileName = originalFileName.Trim();
        FileExtension = fileExtension.Trim();
        MimeType = mimeType.Trim();
        FileSize = fileSize;

        return Result.Success();
    }

    public Result UpdateStorageLocation(string storageProvider, string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storageProvider))
            return Result.Failure(Error.Validation(ErrorCodes.Content.StorageProviderRequired));

        if (string.IsNullOrWhiteSpace(storagePath))
            return Result.Failure(Error.Validation(ErrorCodes.Content.StoragePathRequired));

        StorageProvider = storageProvider.Trim();
        StoragePath = storagePath.Trim();

        return Result.Success();
    }

    public void SetPublicUrl(string? publicUrl) => PublicUrl = publicUrl?.Trim();
    public void EnableDownload() => AllowDownload = true;
    public void DisableDownload() => AllowDownload = false;
}
