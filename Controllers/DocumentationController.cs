using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Project_Echo.Models;

namespace Project_Echo.Controllers
{
    [Route("docs")]
    public class DocumentationController : Controller
    {
        private readonly IHostEnvironment _environment;

        public DocumentationController(IHostEnvironment environment)
        {
            _environment = environment;
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
            
            // Pass the view model to the view
            return View("ViewDocument", viewModel);
        }

        private List<DocumentationLink> GetNavigationLinks()
        {
            return new List<DocumentationLink>
            {
                new DocumentationLink { Title = "Documentation Home", Url = "/index" },
                new DocumentationLink { Title = "Getting Started", Url = "/getting-started" },
                new DocumentationLink { Title = "User Guide", Url = "/user-guide" },
                new DocumentationLink { Title = "Features", Url = "/features" },
                new DocumentationLink { Title = "API Reference", Url = "/api-reference" },
                new DocumentationLink { Title = "Deployment", Url = "/deployment" },
                new DocumentationLink { Title = "Development", Url = "/development" },
                new DocumentationLink { Title = "Troubleshooting", Url = "/troubleshooting" }
            };
        }
    }
} 