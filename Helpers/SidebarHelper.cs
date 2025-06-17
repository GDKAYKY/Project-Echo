namespace Project_Echo.Helpers
{
    public class SidebarLinkInfo
    {
        public string? Href { get; set; }
        public List<string>? DividerSelectors { get; set; }
    }

    public static class SidebarHelper
    {
        private static readonly List<SidebarLinkInfo> _sidebarLinks =
        [
            // Place more specific paths first, especially for documentation
            new SidebarLinkInfo { Href = "/Documentation", DividerSelectors = [".dividers .line-5", ".dividers .line-6"] },
            new SidebarLinkInfo { Href = "/Terminal", DividerSelectors = [".dividers .line-3", ".dividers .line-4"] },
            new SidebarLinkInfo { Href = "/RemoteAccess", DividerSelectors = [".dividers .line-4", ".dividers .line-5"] },
            new SidebarLinkInfo { Href = "/Network", DividerSelectors = [".dividers .line-6", ".dividers .line-7"] },
            new SidebarLinkInfo { Href = "/Index", DividerSelectors = [".dividers .line-2", ".dividers .line-3"] },
            new SidebarLinkInfo { Href = "/", DividerSelectors = [".dividers .line-3", ".dividers .line-2"] }
        ];  

        /// <summary>
        /// Determines if a given sidebar link's Href is active based on the current page path.
        /// </summary>
        /// <param name="sidebarHref">The Href attribute of the sidebar link.</param>
        /// <param name="currentPath">The current URL path of the page.</param>
        /// <returns>True if the sidebar link is active, otherwise false.</returns>
        public static bool IsActivePage(string? sidebarHref, string? currentPath)
        {
            // Normalize paths: lower-case, remove trailing slashes, handle null/empty
            currentPath = currentPath?.ToLowerInvariant().TrimEnd('/');
            sidebarHref = sidebarHref?.ToLowerInvariant().TrimEnd('/');

            if (string.IsNullOrEmpty(currentPath)) currentPath = "/";
            if (string.IsNullOrEmpty(sidebarHref)) sidebarHref = "/";

            // 1. Exact match is always the strongest
            if (sidebarHref == currentPath) return true;

            // 2. Handle root and index page equivalency
            if ((sidebarHref == "/" && currentPath == "/index") || 
                (sidebarHref == "/index" && currentPath == "/"))
            {
                return true;
            }

            // 3. Special handling for documentation links
            if (sidebarHref.Equals("/documentation", StringComparison.InvariantCultureIgnoreCase))
            {
                return currentPath == "/documentation" || 
                       currentPath.StartsWith("/documentation/");
            }

            return false;
        }

        /// <summary>
        /// Gets the active sidebar link information for the current page path.
        /// </summary>
        /// <param name="currentPath">The current URL path of the page.</param>
        /// <returns>A SidebarLinkInfo object for the active link, or null if no active link is found.</returns>
        public static SidebarLinkInfo? GetActiveSidebarState(string? currentPath)
        {
            // Normalize the currentPath by removing query strings, trimming trailing slashes, and making it lowercase.
            var normalizedPath = currentPath?.Split('?')[0].ToLowerInvariant().TrimEnd('/');
            if (string.IsNullOrEmpty(normalizedPath)) normalizedPath = "/";

            // Find the active link based on the IsActivePage logic
            var activeLink = _sidebarLinks.FirstOrDefault(link => IsActivePage(link.Href, normalizedPath));

            // Special handling for documentation pages
            if (activeLink != null && (activeLink.Href?.ToLowerInvariant() == "/documentation" || 
                normalizedPath.StartsWith("/documentation/")))
            {
                activeLink.DividerSelectors = [".dividers .line-5", ".dividers .line-6"];
            }
            // Special handling for the line-1 divider for the root/index page
            else if (activeLink != null && (activeLink.Href == "/" || activeLink.Href == "/Index"))
            {
                if (activeLink.DividerSelectors == null)
                {
                    activeLink.DividerSelectors = [".dividers .line-1"];
                }
                else if (!activeLink.DividerSelectors.Contains(".dividers .line-1"))
                {
                    activeLink.DividerSelectors.Insert(0, ".dividers .line-1");
                }
            }

            return activeLink;
        }
    }
} 