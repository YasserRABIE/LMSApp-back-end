using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;

namespace LMS.Infrastructure.Services;

public class VideoUploadService : IVideoUploadService
{
    public Task<VideoUploadCredentials> GetUploadCredentialsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        // TODO: Implement video upload credentials generation (VdoCipher, Vimeo, etc.)
        throw new NotImplementedException("Video upload credentials not yet implemented");
    }

    public Task<VideoPlaybackInfo> GetPlaybackInfoAsync(string externalVideoId, CancellationToken cancellationToken = default)
    {
        // TODO: Fetch video playback information from provider
        throw new NotImplementedException("Video playback info retrieval not yet implemented");
    }

    public Task<VideoStatus> GetVideoStatusAsync(string externalVideoId, CancellationToken cancellationToken = default)
    {
        // TODO: Check video processing status from provider
        throw new NotImplementedException("Video status check not yet implemented");
    }
}
