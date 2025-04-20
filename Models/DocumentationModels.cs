using System.Collections.Generic;

namespace Project_Echo.Models
{
    public class DocumentationLink
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class DocumentationSection
    {
        public string Title { get; set; } = string.Empty;
        public List<DocumentationLink> Links { get; set; } = new List<DocumentationLink>();
    }

    public class DocumentationViewModel
    {
        public string PageTitle { get; set; } = string.Empty;
        public string PageSubtitle { get; set; } = string.Empty;
        public List<DocumentationSection> Sections { get; set; } = new List<DocumentationSection>();
    }

    public class MarkdownViewModel
    {
        public string DocumentTitle { get; set; } = string.Empty;
        public string MarkdownContent { get; set; } = string.Empty;
        public List<DocumentationLink> NavigationLinks { get; set; } = new List<DocumentationLink>();
    }
} 