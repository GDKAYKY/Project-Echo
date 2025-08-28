using Microsoft.AspNetCore.Mvc;
using Project_Echo.Models;
using Microsoft.Extensions.Logging;

namespace Project_Echo.Controllers;

[Route("Documentation")]
public class DocumentationController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DocumentationController> _logger;

    public DocumentationController(IWebHostEnvironment environment, ILogger<DocumentationController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Documentation index requested");
        return RedirectToAction("ViewDocument", new { document = "index" });
    }

    [HttpGet("{document?}")]
    public async Task<IActionResult> ViewDocument(string document)
    {
        if (string.IsNullOrEmpty(document))
        {
            document = "index";
        }

        // Sanitize input to prevent directory traversal
        document = Path.GetFileNameWithoutExtension(document);
            
        string docPath = Path.Combine(_environment.ContentRootPath, "docs", $"{document}.md");
        _logger.LogInformation("Attempting to load documentation from path: {DocPath}", docPath);
            
        if (!System.IO.File.Exists(docPath))
        {
            _logger.LogWarning("Documentation file not found at path: {DocPath}", docPath);
            return NotFound();
        }

        try
        {
            // Read the markdown content
            string markdownContent = await System.IO.File.ReadAllTextAsync(docPath);
            _logger.LogInformation("Successfully loaded documentation file: {DocPath}", docPath);
                
            // Create view model with initialized properties
            var viewModel = new MarkdownViewModel
            {
                DocumentTitle = char.ToUpper(document[0]) + document.Substring(1).Replace("-", " "),
                MarkdownContent = markdownContent,
                NavigationLinks = GetNavigationLinks()
            };
                
            // Explicitly specify the full path to the view
            return View("~/Views/Documentation/ViewDocument.cshtml", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading documentation file: {DocPath}", docPath);
            return StatusCode(500, "Error loading documentation");
        }
    }

    private static List<DocumentationLink> GetNavigationLinks()
    {
        return
        [
            new DocumentationLink { Title = "Documentation Home", Url = "/Documentation/index" },
            new DocumentationLink { Title = "Getting Started", Url = "/Documentation/getting-started" },
            new DocumentationLink { Title = "User Guide", Url = "/Documentation/user-guide" },
            new DocumentationLink { Title = "Features", Url = "/Documentation/features" },
            new DocumentationLink { Title = "API Reference", Url = "/Documentation/api-reference" },
            new DocumentationLink { Title = "Deployment", Url = "/Documentation/deployment" },
            new DocumentationLink { Title = "Development", Url = "/Documentation/development" },
            new DocumentationLink { Title = "Troubleshooting", Url = "/Documentation/troubleshooting" }
        ];
    }
}