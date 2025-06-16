using Microsoft.AspNetCore.Mvc;
using Project_Echo.Models;

namespace Project_Echo.Controllers
{
    [Route("Documentation")]
    public class DocumentationController(IWebHostEnvironment environment) : Controller
    {
        private readonly IWebHostEnvironment _environment = environment;

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
            
            if (!System.IO.File.Exists(docPath))
            {
                return NotFound();
            }

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