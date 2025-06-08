using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Echo.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Project_Echo.Pages
{
    public class DocumentationViewModel
    {
        public required string PageTitle { get; init; }
        public required string PageSubtitle { get; init; }
        public required List<DocumentationSection> Sections { get; set; }
        public required string CurrentUrl { get; set; }
        public bool IsDocsSection { get; set; }
    }
    public class DocumentationModel : PageModel
    {
        public DocumentationViewModel DocViewModel { get; private set; } = new DocumentationViewModel
        {
            PageTitle = "Documentation",
            PageSubtitle = "Guides and Reference Materials",
            Sections = new List<DocumentationSection>(),
            CurrentUrl = string.Empty,
            IsDocsSection = false
        };

        public void OnGet()
        {
            DocViewModel.CurrentUrl = HttpContext.Request.Path;
            DocViewModel.IsDocsSection = HttpContext.Request.Path.StartsWithSegments("/Documentation/docs") ||
                                        (HttpContext.Request.Path.Value?.StartsWith("/Documentation/docs/") ?? false);
            
            DocViewModel.Sections =
            [
                new DocumentationSection
                {
                    Title = "Getting Started",
                    Links =
                    [
                        new DocumentationLink { Title = "Introduction to ECHO", Url = "/Documentation/docs/introduction" },
                        new DocumentationLink { Title = "Installation Guide", Url = "/Documentation/docs/installation" },
                        new DocumentationLink { Title = "Configuration Options", Url = "/Documentation/docs/configuration" }
                    ]
                },

                new DocumentationSection
                {
                    Title = "Features",
                    Links =
                    [
                        new DocumentationLink { Title = "Database Search", Url = "/Documentation/docs/features/database-search" },
                        new DocumentationLink { Title = "SSH Terminal", Url = "/Documentation/docs/features/ssh-terminal" },
                        new DocumentationLink { Title = "Remote Desktop", Url = "/Documentation/docs/features/remote-desktop" },
                        new DocumentationLink { Title = "Network Management", Url = "/Documentation/docs/features/network-management" }
                    ]
                },

                new DocumentationSection
                {
                    Title = "API Reference",
                    Links =
                    [
                        new DocumentationLink { Title = "REST API", Url = "/Documentation/docs/api/rest" },
                        new DocumentationLink { Title = "GraphQL Schema", Url = "/Documentation/docs/api/graphql" },
                        new DocumentationLink { Title = "Authentication", Url = "/Documentation/docs/api/authentication" }
                    ]
                },

                new DocumentationSection
                {
                    Title = "Complete Documentation",
                    Links =
                    [
                        new DocumentationLink { Title = "Documentation Home", Url = "/Documentation/docs/index" },
                        new DocumentationLink { Title = "Getting Started Guide", Url = "/Documentation/docs/getting-started" },
                        new DocumentationLink { Title = "User Guide", Url = "/Documentation/docs/user-guide" },
                        new DocumentationLink { Title = "Feature Reference", Url = "/Documentation/docs/features" },
                        new DocumentationLink { Title = "API Documentation", Url = "/Documentation/docs/api-reference" },
                        new DocumentationLink { Title = "Deployment Guide", Url = "/Documentation/docs/deployment" },
                        new DocumentationLink { Title = "Developer Guide", Url = "/Documentation/docs/development" },
                        new DocumentationLink { Title = "Troubleshooting", Url = "/Documentation/docs/troubleshooting" }
                    ]
                }
            ];
        }
    }
}