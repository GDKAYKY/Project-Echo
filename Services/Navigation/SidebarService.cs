using Project_Echo.Helpers;
using Project_Echo.Models.Navigation;

namespace Project_Echo.Services.Navigation
{
    public interface ISidebarService
    {
        List<SidebarItem> GetSidebarItems(string currentPath);
    }

    public class SidebarService : ISidebarService
    {
        public List<SidebarItem> GetSidebarItems(string currentPath)
        {
            return
            [
                new SidebarItem
                {
                    Href = "/",
                    IconClass = "sql-query",
                    IconType = "database-search-icon",
                    IsActive = SidebarHelper.IsActivePage("/", currentPath)
                },
                new SidebarItem
                {
                    Href = "/Terminal",
                    IconClass = "ssh-terminal",
                    IconType = "terminal-icon",
                    IsActive = SidebarHelper.IsActivePage("/Terminal", currentPath)
                },
                new SidebarItem
                {
                    Href = "/RemoteAccess",
                    IconClass = "remote-access",
                    IconType = "desktop-windows-icon",
                    IsActive = SidebarHelper.IsActivePage("/RemoteAccess", currentPath)
                },
                new SidebarItem
                {
                    Href = "/Documentation",
                    IconClass = "documentation",
                    IconType = "docs-icon",
                    IsActive = SidebarHelper.IsActivePage("/Documentation", currentPath)
                },
                new SidebarItem
                {
                    Href = "/Network",
                    IconClass = "network",
                    IconType = "public-icon",
                    IsActive = SidebarHelper.IsActivePage("/Network", currentPath)
                }
            ];
        }
    }
} 