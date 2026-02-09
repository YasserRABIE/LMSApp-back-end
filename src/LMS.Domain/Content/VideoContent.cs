using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents video-specific content stored in VdoCipher or similar providers
/// </summary>
public sealed class VideoContent : Entity<Guid>
{
    /// <summary>
    /// Reference to the parent content item
    /// </summary>
    public Guid ContentItemId { get; private set; }

    /// <summary>
    /// Video provider identifier (e.g., "VdoCipher", "Vimeo")
    /// </summary>
    public string ProviderId { get; private set; }

    /// <summary>
    /// External video ID from the provider
    /// </summary>
    public string ExternalVideoId { get; private set; }

    /// <summary>
    /// Video duration in seconds
    /// </summary>
    public int DurationSeconds { get; private set; }

    /// <summary>
    /// Original uploaded file name
    /// </summary>
    public string? OriginalFileName { get; private set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSize { get; private set; }

    /// <summary>
    /// Video resolution (e.g., "1080p", "720p")
    /// </summary>
    public string? Resolution { get; private set; }

    /// <summary>
    /// Processing status of the video
    /// </summary>
    public VideoStatus Status { get; private set; }

    /// <summary>
    /// URL to video transcript/subtitles if available
    /// </summary>
    public string? TranscriptUrl { get; private set; }

    // EF Core constructor
    private VideoContent() : base()
    {
        ProviderId = string.Empty;
        ExternalVideoId = string.Empty;
    }

    private VideoContent(
        Guid id,
        Guid contentItemId,
        string providerId,
        string externalVideoId,
        int durationSeconds,
        string? originalFileName = null,
        long? fileSize = null,
        string? resolution = null)
        : base(id)
    {
        ContentItemId = contentItemId;
        ProviderId = providerId;
        ExternalVideoId = externalVideoId;
        DurationSeconds = durationSeconds;
        OriginalFileName = originalFileName;
        FileSize = fileSize;
        Resolution = resolution;
        Status = VideoStatus.Processing;
    }

    /// <summary>
    /// Factory method to create a new video content entry
    /// </summary>
    public static Result<VideoContent> Create(
        Guid contentItemId,
        string providerId,
        string externalVideoId,
        int durationSeconds,
        string? originalFileName = null,
        long? fileSize = null,
        string? resolution = null)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<VideoContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (string.IsNullOrWhiteSpace(providerId))
        {
            return Result<VideoContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (providerId.Length > 50)
        {
            return Result<VideoContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (string.IsNullOrWhiteSpace(externalVideoId))
        {
            return Result<VideoContent>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (externalVideoId.Length > 500)
        {
            return Result<VideoContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        if (durationSeconds <= 0)
        {
            return Result<VideoContent>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var videoContent = new VideoContent(
            Guid.NewGuid(),
            contentItemId,
            providerId,
            externalVideoId,
            durationSeconds,
            originalFileName,
            fileSize,
            resolution
        );

        return Result<VideoContent>.Success(videoContent);
    }

    /// <summary>
    /// Updates video metadata
    /// </summary>
    public Result UpdateMetadata(
        int durationSeconds,
        string? originalFileName,
        long? fileSize,
        string? resolution)
    {
        if (durationSeconds <= 0)
        {
            return Result.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        DurationSeconds = durationSeconds;
        OriginalFileName = originalFileName;
        FileSize = fileSize;
        Resolution = resolution;

        return Result.Success();
    }

    /// <summary>
    /// Marks the video as ready for playback
    /// </summary>
    public Result MarkAsReady()
    {
        if (Status == VideoStatus.Ready)
        {
            return Result.Failure(
                Error.Conflict("VIDEO.ALREADY_READY")
            );
        }

        Status = VideoStatus.Ready;
        return Result.Success();
    }

    /// <summary>
    /// Marks the video processing as failed
    /// </summary>
    public void MarkAsFailed() => Status = VideoStatus.Failed;

    /// <summary>
    /// Resets video to processing state (for retry scenarios)
    /// </summary>
    public void ResetToProcessing() => Status = VideoStatus.Processing;

    /// <summary>
    /// Sets the transcript URL
    /// </summary>
    public void SetTranscript(string? transcriptUrl) => TranscriptUrl = transcriptUrl;
}
