using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Echo.Services.Media;

namespace Project_Echo.Pages
{
    public class ConvertModel : PageModel
    {
        private readonly IMediaConversionService _mediaConversionService;

        public ConvertModel(IMediaConversionService mediaConversionService)
        {
            _mediaConversionService = mediaConversionService;
        }

        [BindProperty]
        public string? Url { get; set; }

        [BindProperty]
        public string? DownloadType { get; set; }

        [BindProperty]
        public string? Format { get; set; }

        [BindProperty]
        public IFormFile? Upload { get; set; }

        [BindProperty]
        public string? TargetFormat { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostDownload()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                ErrorMessage = "URL is required.";
                return Page();
            }

            var safeFormat = NormalizeFormat(Format);
            var isAudio = string.Equals(DownloadType, "audio", StringComparison.OrdinalIgnoreCase);

            try
            {
                var result = await _mediaConversionService.DownloadFromUrlAsync(Url!, safeFormat, isAudio, HttpContext.RequestAborted);
                return File(result.Content, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConvert()
        {
            if (Upload == null || Upload.Length == 0)
            {
                ErrorMessage = "A file is required.";
                return Page();
            }

            var safeTarget = NormalizeFormat(TargetFormat);

            try
            {
                await using var inputStream = Upload.OpenReadStream();
                var result = await _mediaConversionService.ConvertLocalAsync(Upload.FileName, inputStream, safeTarget, HttpContext.RequestAborted);
                return File(result.Content, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        private static string NormalizeFormat(string? format)
        {
            var value = string.IsNullOrWhiteSpace(format) ? "mp4" : format.Trim().ToLowerInvariant();
            var allow = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "mp4", "webm", "mp3", "opus", "wav", "ogg", "best"
            };
            return allow.Contains(value) ? value : "mp4";
        }
    }
}


