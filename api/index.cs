using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace Project_Echo.Api
{
    public class Handler
    {
        public static IActionResult Run(HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            name = string.IsNullOrEmpty(name) ? "Project Echo" : name;

            var responseMessage = $"Hello, {name}! Welcome to Project Echo API";
            return new OkObjectResult(responseMessage);
        }
    }
} 