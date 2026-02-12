using LMS.Domain.Content;

namespace LMS.Application.Common.Interfaces;

public interface IVideoUploadService
{
    Task<VideoUploadCredentials> GetUploadCredentialsAsync(string fileName, CancellationToken cancellationToken = default);
    Task<VideoPlaybackInfo> GetPlaybackInfoAsync(string externalVideoId, CancellationToken cancellationToken = default);
    Task<VideoStatus> GetVideoStatusAsync(string externalVideoId, CancellationToken cancellationToken = default);
}

public record VideoUploadCredentials(string UploadUrl, string ExternalVideoId, Dictionary<string, string> Headers);
public record VideoPlaybackInfo(string PlaybackUrl, string ExternalVideoId, int DurationSeconds, string? Poster);
