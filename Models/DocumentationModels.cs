namespace Project_Echo.Models
{
    public class DocumentationLink
    {
        public required string Title { get; set; }
        public required string Url { get; set; }
    }

    public class DocumentationSection
    {
        public required string Title { get; set; }
        public required List<DocumentationLink> Links { get; set; }
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