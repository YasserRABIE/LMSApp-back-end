using LMS.Domain.Common;

namespace LMS.Domain.Content;

public sealed class VideoContent : Entity<Guid>
{
    public ContentItemId ContentItemId { get; private set; }
    public string ProviderId { get; private set; }
    public string ExternalVideoId { get; private set; }
    public int DurationSeconds { get; private set; }
    public string? OriginalFileName { get; private set; }
    public long? FileSize { get; private set; }
    public string? Resolution { get; private set; }
    public VideoStatus Status { get; private set; }
    public string? TranscriptUrl { get; private set; }

    private VideoContent() : base()
    {
        ContentItemId = null!;
        ProviderId = string.Empty;
        ExternalVideoId = string.Empty;
    }

    private VideoContent(
        Guid id,
        ContentItemId contentItemId,
        string providerId,
        string externalVideoId,
        int durationSeconds,
        string? originalFileName,
        long? fileSize,
        string? resolution) : base(id)
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

    public static Result<VideoContent> Create(
        ContentItemId contentItemId,
        string providerId,
        string externalVideoId,
        int durationSeconds,
        string? originalFileName = null,
        long? fileSize = null,
        string? resolution = null)
    {
        if (string.IsNullOrWhiteSpace(providerId))
            return Result<VideoContent>.Failure(Error.Validation(ErrorCodes.Content.ProviderIdRequired));

        if (providerId.Length > 50)
            return Result<VideoContent>.Failure(Error.Validation(ErrorCodes.Content.ProviderIdTooLong));

        if (string.IsNullOrWhiteSpace(externalVideoId))
            return Result<VideoContent>.Failure(Error.Validation(ErrorCodes.Content.ExternalVideoIdRequired));

        if (externalVideoId.Length > 500)
            return Result<VideoContent>.Failure(Error.Validation(ErrorCodes.Content.ExternalVideoIdTooLong));

        if (durationSeconds <= 0)
            return Result<VideoContent>.Failure(Error.Validation(ErrorCodes.Content.InvalidDuration));

        var videoContent = new VideoContent(
            Guid.NewGuid(),
            contentItemId,
            providerId.Trim(),
            externalVideoId.Trim(),
            durationSeconds,
            originalFileName?.Trim(),
            fileSize,
            resolution?.Trim());

        return Result<VideoContent>.Success(videoContent);
    }

    public Result UpdateMetadata(int durationSeconds, string? originalFileName, long? fileSize, string? resolution)
    {
        if (durationSeconds <= 0)
            return Result.Failure(Error.Validation(ErrorCodes.Content.InvalidDuration));

        DurationSeconds = durationSeconds;
        OriginalFileName = originalFileName?.Trim();
        FileSize = fileSize;
        Resolution = resolution?.Trim();

        return Result.Success();
    }

    public Result MarkAsReady()
    {
        if (Status == VideoStatus.Ready)
            return Result.Failure(Error.Conflict(ErrorCodes.Content.VideoAlreadyReady));

        Status = VideoStatus.Ready;
        return Result.Success();
    }

    public void MarkAsFailed() => Status = VideoStatus.Failed;
    public void ResetToProcessing() => Status = VideoStatus.Processing;
    public void SetTranscript(string? transcriptUrl) => TranscriptUrl = transcriptUrl?.Trim();
}
