using Microsoft.AspNetCore.Mvc;

namespace ProjectEcho.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Base64Controller : ControllerBase
{
    [HttpPost("encode")]
    public async Task<IActionResult> EncodeImageToBase64(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No File Sent." });
            }

            // Verificar se é uma imagem válida
            var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
            "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp",
            "video/mp4", "video/webm", "video/ogg", "video/avi", "video/mov", "video/mkv"
            };

            if (!allowedTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Unsuported File Format." });
            }

            // Max Base64 File Size 128MB
            if (file.Length > 128 * 1024 * 1024)
            {
                return BadRequest(new { error = "Max File Size Reached." });
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);

                return Ok(new
                {
                    base64 = base64String,
                    mimeType = file.ContentType,
                    fileName = file.FileName,
                    size = file.Length,
                    sizeFormatted = FormatBytes(file.Length),
                    dataUrl = $"data:{file.ContentType};base64,{base64String}"
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Internal Server Error.", details = ex.Message });
        }
    }
    [HttpPost("decode")]
    public IActionResult DecodeBase64ToImage([FromBody] Base64DecodeRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Base64Data))
            {
                return BadRequest(new { error = "Couldnt find Base64 Data" });
            }

            // Remover prefixo data URL se presente
            string base64Data = request.Base64Data ?? "";

            int commaIndex = base64Data.IndexOf(',');
            if (commaIndex >= 0)
                base64Data = base64Data[(commaIndex + 1)..];

            base64Data = base64Data.Trim();

            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Invalid Base64");
            }


            // Determinar tipo de conteúdo
            string contentType = request.MimeType ?? "image/png";
            string fileName = request.FileName ?? $"image_{DateTime.Now:yyyyMMddHHmmss}.png";

            return File(imageBytes, contentType, fileName);
        }
        catch (FormatException)
        {
            return BadRequest(new { error = "Formato Base64 inválido." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno do servidor.", details = ex.Message });
        }
    }


    [HttpPost("validate")]
    public IActionResult ValidateBase64([FromBody] Base64ValidateRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Base64Data))
            {
                return Ok(new { isValid = false, error = "Base64 Data not Provided." });
            }

            var base64Data = request.Base64Data;
            if (base64Data.Contains(','))
            {
                base64Data = base64Data.Split(',')[1];
            }

            var imageBytes = Convert.FromBase64String(base64Data);

            return Ok(new
            {
                isValid = true,
                size = imageBytes.Length,
                sizeFormatted = FormatBytes(imageBytes.Length)
            });
        }
        catch (FormatException)
        {
            return Ok(new { isValid = false, error = "Invalid Base64 Format." });
        }
        catch (Exception ex)
        {
            return Ok(new { isValid = false, error = ex.Message });
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] suffixes = ["B", "KB", "MB", "GB"];
        int counter = 0;
        decimal number = bytes;
        while (Math.Round(number / 1024) >= 1 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }
        return string.Format("{0:n1} {1}", number, suffixes[counter]);
    }
}

public class Base64DecodeRequest
{
    public string? Base64Data { get; set; }
    public string? MimeType { get; set; }
    public string? FileName { get; set; }
}

public class Base64ValidateRequest
{
    public string? Base64Data { get; set; }
}