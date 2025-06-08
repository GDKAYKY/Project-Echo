namespace Project_Echo.Models.Navigation
{
    public class SidebarItem
    {
        public string Href { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string IconType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
} 