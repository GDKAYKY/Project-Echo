using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Echo.Models;
using System.Collections.Generic;

namespace Project_Echo.Pages
{
    public class DocumentationModel : PageModel
    {
        public DocumentationViewModel DocViewModel { get; private set; } = new DocumentationViewModel
        {
            PageTitle = "Documentation",
            PageSubtitle = "Guides and Reference Materials",
            Sections = new List<DocumentationSection>()
        };

        public void OnGet()
        {
            DocViewModel.Sections = new List<DocumentationSection>
            {
                new DocumentationSection
                {
                    Title = "Getting Started",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "Introduction to ECHO", Url = "#" },
                        new DocumentationLink { Title = "Installation Guide", Url = "#" },
                        new DocumentationLink { Title = "Configuration Options", Url = "#" }
                    }
                },
                new DocumentationSection
                {
                    Title = "Features",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "Database Search", Url = "#" },
                        new DocumentationLink { Title = "SSH Terminal", Url = "#" },
                        new DocumentationLink { Title = "Remote Desktop", Url = "#" },
                        new DocumentationLink { Title = "Network Management", Url = "#" }
                    }
                },
                new DocumentationSection
                {
                    Title = "API Reference",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "REST API", Url = "#" },
                        new DocumentationLink { Title = "GraphQL Schema", Url = "#" },
                        new DocumentationLink { Title = "Authentication", Url = "#" }
                    }
                },
                new DocumentationSection
                {
                    Title = "Complete Documentation",
                    Links = new List<DocumentationLink>
                    {
                        new DocumentationLink { Title = "Documentation Home", Url = "/docs/index" },
                        new DocumentationLink { Title = "Getting Started Guide", Url = "/docs/getting-started" },
                        new DocumentationLink { Title = "User Guide", Url = "/docs/user-guide" },
                        new DocumentationLink { Title = "Feature Reference", Url = "/docs/features" },
                        new DocumentationLink { Title = "API Documentation", Url = "/docs/api-reference" },
                        new DocumentationLink { Title = "Deployment Guide", Url = "/docs/deployment" },
                        new DocumentationLink { Title = "Developer Guide", Url = "/docs/development" },
                        new DocumentationLink { Title = "Troubleshooting", Url = "/docs/troubleshooting" }
                    }
                }
            };
        }
    }
} 