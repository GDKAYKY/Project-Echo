using Microsoft.AspNetCore.Mvc;
using Project_Echo.Models;
using Microsoft.Extensions.Logging;

namespace Project_Echo.Controllers
{
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
            return RedirectToAction("ViewDocument", new { document = "index" });
        }

        [HttpGet("docs/{document?}")]
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

        private List<DocumentationLink> GetNavigationLinks()
        {
            return
            [
                new() { Title = "Documentation Home", Url = "/Documentation/docs/index" },
                new() { Title = "Getting Started", Url = "/Documentation/docs/getting-started" },
                new() { Title = "User Guide", Url = "/Documentation/docs/user-guide" },
                new() { Title = "Features", Url = "/Documentation/docs/features" },
                new() { Title = "API Reference", Url = "/Documentation/docs/api-reference" },
                new() { Title = "Deployment", Url = "/Documentation/docs/deployment" },
                new() { Title = "Development", Url = "/Documentation/docs/development" },
                new() { Title = "Troubleshooting", Url = "/Documentation/docs/troubleshooting" }
            ];
        }
    }
} 