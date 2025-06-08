using Microsoft.AspNetCore.Mvc;
using Project_Echo.Models;

namespace Project_Echo.Controllers
{
    [Route("Documentation/docs")]
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
            
            // Explicitly specify the full path to the view
            return View("~/Views/Documentation/ViewDocument.cshtml", viewModel);
        }

        private List<DocumentationLink> GetNavigationLinks()
        {
            return new List<DocumentationLink>
            {
                new DocumentationLink { Title = "Documentation Home", Url = "/Documentation/docs/index" },
                new DocumentationLink { Title = "Getting Started", Url = "/Documentation/docs/getting-started" },
                new DocumentationLink { Title = "User Guide", Url = "/Documentation/docs/user-guide" },
                new DocumentationLink { Title = "Features", Url = "/Documentation/docs/features" },
                new DocumentationLink { Title = "API Reference", Url = "/Documentation/docs/api-reference" },
                new DocumentationLink { Title = "Deployment", Url = "/Documentation/docs/deployment" },
                new DocumentationLink { Title = "Development", Url = "/Documentation/docs/development" },
                new DocumentationLink { Title = "Troubleshooting", Url = "/Documentation/docs/troubleshooting" }
            };
        }

        private List<DocumentationSection> GetDocumentationSections()
        {
            return new List<DocumentationSection>
            {
                new DocumentationSection
                {
                    Title = "Getting Started",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "Introduction to ECHO", Url = "/Documentation/docs/introduction" },
                        new DocumentationLink { Title = "Installation Guide", Url = "/Documentation/docs/installation" },
                        new DocumentationLink { Title = "Configuration Options", Url = "/Documentation/docs/configuration" }
                    }
                },
                new DocumentationSection
                {
                    Title = "Features",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "Database Search", Url = "/Documentation/docs/database-search" },
                        new DocumentationLink { Title = "SSH Terminal", Url = "/Documentation/docs/ssh-terminal" },
                        new DocumentationLink { Title = "Remote Desktop", Url = "/Documentation/docs/remote-desktop" },
                        new DocumentationLink { Title = "Network Management", Url = "/Documentation/docs/network-management" }
                    }
                },
                new DocumentationSection
                {
                    Title = "API Reference",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "REST API", Url = "/Documentation/docs/rest-api" },
                        new DocumentationLink { Title = "GraphQL Schema", Url = "/Documentation/docs/graphql-schema" },
                        new DocumentationLink { Title = "Authentication", Url = "/Documentation/docs/authentication" }
                    }
                },
                new DocumentationSection
                {
                    Title = "Complete Documentation",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "Documentation Home", Url = "/Documentation/docs/index" },
                        new DocumentationLink { Title = "Getting Started Guide", Url = "/Documentation/docs/getting-started" },
                        new DocumentationLink { Title = "User Guide", Url = "/Documentation/docs/user-guide" },
                        new DocumentationLink { Title = "Feature Reference", Url = "/Documentation/docs/features" },
                        new DocumentationLink { Title = "API Documentation", Url = "/Documentation/docs/api-reference" },
                        new DocumentationLink { Title = "Deployment Guide", Url = "/Documentation/docs/deployment" },
                        new DocumentationLink { Title = "Developer Guide", Url = "/Documentation/docs/development" },
                        new DocumentationLink { Title = "Troubleshooting", Url = "/Documentation/docs/troubleshooting" }
                    }
                }
            };
        }
    }
} 