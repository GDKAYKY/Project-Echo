using System.Diagnostics;
using System.Text;

namespace Project_Echo.Services.Media
{
    public sealed class MediaConversionService : IMediaConversionService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MediaConversionService> _logger;

        public MediaConversionService(IWebHostEnvironment env, ILogger<MediaConversionService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<MediaResult> DownloadFromUrlAsync(string url, string format, bool audioOnly, CancellationToken cancellationToken)
        {
            var tempRoot = GetTempRoot();
            Directory.CreateDirectory(tempRoot);

            var safeBase = SanitizeFileName(Guid.NewGuid().ToString("n"));
            var outputPathNoExt = Path.Combine(tempRoot, safeBase);

            var args = BuildYtDlpArgs(url, format, audioOnly, outputPathNoExt);
            await RunProcessAsync("yt-dlp", args, cancellationToken);

            var filePath = ResolveOutputPath(tempRoot, safeBase, format);

            var content = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var result = new MediaResult
            {
                Content = content,
                ContentType = GetContentTypeFromExtension(Path.GetExtension(filePath)),
                FileName = Path.GetFileName(filePath)
            };

            TryDelete(filePath);
            return result;
        }

        public async Task<MediaResult> ConvertLocalAsync(string originalFileName, Stream inputStream, string targetFormat, CancellationToken cancellationToken)
        {
            var tempRoot = GetTempRoot();
            Directory.CreateDirectory(tempRoot);

            var inputPath = Path.Combine(tempRoot, SanitizeFileName(Guid.NewGuid().ToString("n") + Path.GetExtension(originalFileName)));
            await using (var fs = File.Create(inputPath))
            {
                await inputStream.CopyToAsync(fs, cancellationToken);
            }

            var outputPath = Path.Combine(tempRoot, SanitizeFileName(Path.GetFileNameWithoutExtension(inputPath)) + "." + targetFormat);

            var args = new[]
            {
                "-y",
                "-i", Quote(inputPath),
                Quote(outputPath)
            };

            await RunProcessAsync("ffmpeg", string.Join(' ', args), cancellationToken);

            var content = await File.ReadAllBytesAsync(outputPath, cancellationToken);
            var result = new MediaResult
            {
                Content = content,
                ContentType = GetContentTypeFromExtension(Path.GetExtension(outputPath)),
                FileName = Path.GetFileName(outputPath)
            };

            TryDelete(inputPath);
            TryDelete(outputPath);
            return result;
        }

        private static string BuildYtDlpArgs(string url, string format, bool audioOnly, string outputPathNoExt)
        {
            var sb = new StringBuilder();
            sb.Append("-o ").Append(Quote(outputPathNoExt + ".%(ext)s"));
            if (audioOnly)
            {
                sb.Append(" -x");
                if (!string.Equals(format, "best", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" --audio-format ").Append(format);
                }
            }
            else
            {
                if (string.Equals(format, "best", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" -f bestvideo+bestaudio/best");
                }
                else
                {
                    // Prefer best video+audio and remux to requested container if needed
                    sb.Append(" -f bestvideo+bestaudio/best");
                    sb.Append(" --remux-video ").Append(format);
                }
            }
            sb.Append(' ').Append(Quote(url));
            return sb.ToString();
        }

        private static string ResolveOutputPath(string tempRoot, string baseName, string requestedFormat)
        {
            // If best, the extension is decided by yt-dlp; otherwise prefer requested format
            if (!string.Equals(requestedFormat, "best", StringComparison.OrdinalIgnoreCase))
            {
                var candidate = Path.Combine(tempRoot, baseName + "." + requestedFormat);
                if (File.Exists(candidate)) return candidate;
            }

            var files = Directory.GetFiles(tempRoot, baseName + ".*");
            if (files.Length == 0)
                throw new FileNotFoundException("Download finished but output file was not found.");
            return files[0];
        }

        private static string Quote(string value) => "\"" + value.Replace("\"", "\\\"") + "\"";

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        private string GetTempRoot()
        {
            var root = Path.Combine(_env.ContentRootPath, "database_storage", "tmp");
            return root;
        }

        private static string GetContentTypeFromExtension(string ext)
        {
            ext = (ext ?? string.Empty).Trim('.').ToLowerInvariant();
            return ext switch
            {
                "mp4" => "video/mp4",
                "webm" => "video/webm",
                "mp3" => "audio/mpeg",
                "opus" => "audio/opus",
                "wav" => "audio/wav",
                "ogg" => "audio/ogg",
                _ => "application/octet-stream"
            };
        }

        private async Task RunProcessAsync(string fileName, string arguments, CancellationToken cancellationToken)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            process.OutputDataReceived += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) _logger.LogInformation("{Tool}: {Line}", fileName, e.Data); };
            process.ErrorDataReceived += (_, e) => { if (!string.IsNullOrEmpty(e.Data)) _logger.LogInformation("{Tool} ERR: {Line}", fileName, e.Data); };
            process.Exited += (_, __) => tcs.TrySetResult(process.ExitCode);

            if (!process.Start()) throw new InvalidOperationException($"Failed to start {fileName}");
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using (cancellationToken.Register(() =>
            {
                try { if (!process.HasExited) process.Kill(true); } catch { }
                tcs.TrySetCanceled();
            }))
            {
                var exitCode = await tcs.Task;
                if (exitCode != 0)
                {
                    throw new InvalidOperationException($"{fileName} exited with code {exitCode}.");
                }
            }
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { }
        }
    }
}


