using System.IO;

namespace Project_Echo.Services.Media
{
    public interface IMediaConversionService
    {
        Task<MediaResult> DownloadFromUrlAsync(string url, string format, bool audioOnly, CancellationToken cancellationToken);
        Task<MediaResult> ConvertLocalAsync(string originalFileName, Stream inputStream, string targetFormat, CancellationToken cancellationToken);
    }

    public sealed class MediaResult
    {
        public required byte[] Content { get; init; }
        public required string ContentType { get; init; }
        public required string FileName { get; init; }
    }
}


