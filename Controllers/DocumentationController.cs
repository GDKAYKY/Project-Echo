using Microsoft.AspNetCore.Mvc;
using Project_Echo.Models;

namespace Project_Echo.Controllers
{
    [Route("Documentation/docs")]
    public class DocumentationController(IHostEnvironment environment) : Controller
    {
        private readonly IHostEnvironment _environment = environment;

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

        private List<DocumentationSection> GetDocumentationSections()
        {
            return
            [
                new() {
                    Title = "Getting Started",
                    Links =
                    [
                        new() { Title = "Introduction to ECHO", Url = "/Documentation/docs/introduction" },
                        new() { Title = "Installation Guide", Url = "/Documentation/docs/installation" },
                        new() { Title = "Configuration Options", Url = "/Documentation/docs/configuration" }
                    ]
                },
                new() {
                    Title = "Features",
                    Links =
                    [
                        new() { Title = "Database Search", Url = "/Documentation/docs/database-search" },
                        new() { Title = "SSH Terminal", Url = "/Documentation/docs/ssh-terminal" },
                        new() { Title = "Remote Desktop", Url = "/Documentation/docs/remote-desktop" },
                        new() { Title = "Network Management", Url = "/Documentation/docs/network-management" }
                    ]
                },
                new() {
                    Title = "API Reference",
                    Links =
                    [
                        new() { Title = "REST API", Url = "/Documentation/docs/rest-api" },
                        new() { Title = "GraphQL Schema", Url = "/Documentation/docs/graphql-schema" },
                        new() { Title = "Authentication", Url = "/Documentation/docs/authentication" }
                    ]
                },
                new() {
                    Title = "Complete Documentation",
                    Links =
                    [
                        new() { Title = "Documentation Home", Url = "/Documentation/docs/index" },
                        new() { Title = "Getting Started Guide", Url = "/Documentation/docs/getting-started" },
                        new() { Title = "User Guide", Url = "/Documentation/docs/user-guide" },
                        new() { Title = "Feature Reference", Url = "/Documentation/docs/features" },
                        new() { Title = "API Documentation", Url = "/Documentation/docs/api-reference" },
                        new() { Title = "Deployment Guide", Url = "/Documentation/docs/deployment" },
                        new() { Title = "Developer Guide", Url = "/Documentation/docs/development" },
                        new() { Title = "Troubleshooting", Url = "/Documentation/docs/troubleshooting" }
                    ]
                }
            ];
        }
    }
} 