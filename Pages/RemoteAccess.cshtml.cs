using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class RemoteAccessModel(ILogger<RemoteAccessModel> logger) : PageModel
    {
        public readonly ILogger<RemoteAccessModel> _logger = logger;
    }
} 