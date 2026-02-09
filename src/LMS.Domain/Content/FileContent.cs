using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents file-based content stored in S3 or similar storage
/// </summary>
public sealed class FileContent : Entity<Guid>
{
    /// <summary>
    /// Reference to the parent content item
    /// </summary>
    public Guid ContentItemId { get; private set; }

    /// <summary>
    /// Storage provider (e.g., "s3", "azure_blob")
    /// </summary>
    public string StorageProvider { get; private set; }

    /// <summary>
    /// Path/key in the storage provider
    /// </summary>
    public string StoragePath { get; private set; }

    /// <summary>
    /// Public URL for accessing the file (if available)
    /// </summary>
    public string? PublicUrl { get; private set; }

    /// <summary>
    /// Original file name as uploaded
    /// </summary>
    public string OriginalFileName { get; private set; }

    /// <summary>
    /// File extension (e.g., ".pdf", ".docx")
    /// </summary>
    public string FileExtension { get; private set; }

    /// <summary>
    /// MIME type (e.g., "application/pdf")
    /// </summary>
    public string MimeType { get; private set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// Indicates if students are allowed to download this file
    /// </summary>
    public bool AllowDownload { get; private set; }

    // EF Core constructor
    private FileContent() : base()
    {
        StorageProvider = string.Empty;
        StoragePath = string.Empty;
        OriginalFileName = string.Empty;
        FileExtension = string.Empty;
        MimeType = string.Empty;
    }

    private FileContent(
        Guid id,
        Guid contentItemId,
        string storageProvider,
        string storagePath,
        string originalFileName,
        string fileExtension,
        string mimeType,
        long fileSize,
        string? publicUrl = null,
        bool allowDownload = false)
        : base(id)
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

    /// <summary>
    /// Factory method to create a new file content entry
    /// </summary>
    public static Result<FileContent> Create(
        Guid contentItemId,
        string storageProvider,
        string storagePath,
        string originalFileName,
        string fileExtension,
        string mimeType,
        long fileSize,
        string? publicUrl = null,
        bool allowDownload = false)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(storageProvider))
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (storageProvider.Length > 20)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (storagePath.Length > 1000)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (originalFileName.Length > 300)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (fileExtension.Length > 20)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (mimeType.Length > 100)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (fileSize <= 0)
        {
            return Result<FileContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var fileContent = new FileContent(
            Guid.NewGuid(),
            contentItemId,
            storageProvider,
            storagePath,
            originalFileName,
            fileExtension,
            mimeType,
            fileSize,
            publicUrl,
            allowDownload
        );

        return Result<FileContent>.Success(fileContent);
    }

    /// <summary>
    /// Updates file metadata
    /// </summary>
    public Result UpdateMetadata(
        string originalFileName,
        string fileExtension,
        string mimeType,
        long fileSize)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (originalFileName.Length > 300)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (fileSize <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        OriginalFileName = originalFileName;
        FileExtension = fileExtension;
        MimeType = mimeType;
        FileSize = fileSize;

        return Result.Success();
    }

    /// <summary>
    /// Sets the public URL for the file
    /// </summary>
    public void SetPublicUrl(string? publicUrl) => PublicUrl = publicUrl;

    /// <summary>
    /// Enables file download for students
    /// </summary>
    public void EnableDownload() => AllowDownload = true;

    /// <summary>
    /// Disables file download for students
    /// </summary>
    public void DisableDownload() => AllowDownload = false;

    /// <summary>
    /// Updates the storage location
    /// </summary>
    public Result UpdateStorageLocation(string storageProvider, string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storageProvider))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        StorageProvider = storageProvider;
        StoragePath = storagePath;

        return Result.Success();
    }
}
