using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Echo.Models;

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
            DocViewModel.IsDocsSection = HttpContext.Request.Path.Value?.StartsWith("/Documentation", StringComparison.OrdinalIgnoreCase) ?? false;
            
            DocViewModel.Sections =
            [
                new DocumentationSection
                {
                    Title = "Getting Started",
                    Links =
                    [
                        new DocumentationLink { Title = "Documentation Home", Url = "/Documentation/index" },
                        new DocumentationLink { Title = "Getting Started Guide", Url = "/Documentation/getting-started" },
                        new DocumentationLink { Title = "User Guide", Url = "/Documentation/user-guide" }
                    ]
                },

                new DocumentationSection
                {
                    Title = "Features & API",
                    Links =
                    [
                        new DocumentationLink { Title = "Features Overview", Url = "/Documentation/features" },
                        new DocumentationLink { Title = "API Reference", Url = "/Documentation/api-reference" }
                    ]
                },

                new DocumentationSection
                {
                    Title = "Development & Deployment",
                    Links =
                    [
                        new DocumentationLink { Title = "Development Guide", Url = "/Documentation/development" },
                        new DocumentationLink { Title = "Deployment Guide", Url = "/Documentation/deployment" },
                        new DocumentationLink { Title = "Troubleshooting", Url = "/Documentation/troubleshooting" }
                    ]
                }
            ];
        }
    }
}