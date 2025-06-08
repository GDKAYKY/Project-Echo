using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class NetworkModel(ILogger<NetworkModel> logger) : PageModel
    {
        public readonly ILogger<NetworkModel> _logger = logger;
    }
} 