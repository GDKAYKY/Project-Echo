using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Project_Echo.Api
{
    public class EchoHandler
    {
        public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
        {
            log.LogInformation("Echo API endpoint processed a request");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (string.IsNullOrEmpty(requestBody))
            {
                return new OkObjectResult(new { message = "No message to echo. Send a JSON body to see it echoed back." });
            }
            
            try
            {
                // Try to parse as JSON
                var jsonDoc = JsonDocument.Parse(requestBody);
                return new OkObjectResult(jsonDoc);
            }
            catch (JsonException)
            {
                // If not valid JSON, just echo back as text
                return new OkObjectResult(new { message = requestBody });
            }
        }
    }
} 