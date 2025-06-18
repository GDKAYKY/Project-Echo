using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ProjectEcho.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Base64Controller : ControllerBase
    {
        /// <summary>
        /// Converte uma imagem enviada para Base64
        /// </summary>
        /// <param name="file">Arquivo de imagem</param>
        /// <returns>String Base64 da imagem</returns>
        [HttpPost("encode")]
        public async Task<IActionResult> EncodeImageToBase64(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "Nenhum arquivo foi enviado." });
                }

                // Verificar se é uma imagem válida
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { error = "Tipo de arquivo não suportado. Use JPEG, PNG, GIF, BMP ou WebP." });
                }

                // Verificar tamanho do arquivo (máximo 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { error = "Arquivo muito grande. Tamanho máximo: 10MB." });
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
                return StatusCode(500, new { error = "Erro interno do servidor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Decodifica uma string Base64 para imagem
        /// </summary>
        /// <param name="request">Dados da requisição contendo Base64</param>
        /// <returns>Arquivo de imagem</returns>
        [HttpPost("decode")]
        public IActionResult DecodeBase64ToImage([FromBody] Base64DecodeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Base64Data))
                {
                    return BadRequest(new { error = "Dados Base64 não fornecidos." });
                }

                // Remover prefixo data URL se presente
                var base64Data = request.Base64Data;
                if (base64Data.Contains(","))
                {
                    base64Data = base64Data.Split(',')[1];
                }

                var imageBytes = Convert.FromBase64String(base64Data);
                
                // Determinar tipo de conteúdo
                var contentType = request.MimeType ?? "image/png";
                var fileName = request.FileName ?? $"image_{DateTime.Now:yyyyMMddHHmmss}.png";

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

        /// <summary>
        /// Valida se uma string é um Base64 válido
        /// </summary>
        /// <param name="request">Dados da requisição</param>
        /// <returns>Resultado da validação</returns>
        [HttpPost("validate")]
        public IActionResult ValidateBase64([FromBody] Base64ValidateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Base64Data))
                {
                    return Ok(new { isValid = false, error = "Dados Base64 não fornecidos." });
                }

                var base64Data = request.Base64Data;
                if (base64Data.Contains(","))
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
                return Ok(new { isValid = false, error = "Formato Base64 inválido." });
            }
            catch (Exception ex)
            {
                return Ok(new { isValid = false, error = ex.Message });
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }
    }

    public class Base64DecodeRequest
    {
        public string Base64Data { get; set; }
        public string? MimeType { get; set; }
        public string? FileName { get; set; }
    }

    public class Base64ValidateRequest
    {
        public string Base64Data { get; set; }
    }
}

