using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class NetworkModel : PageModel
    {
        public readonly ILogger<NetworkModel> _logger;

        public NetworkModel(ILogger<NetworkModel> logger)
        {
            _logger = logger;
        }
        
    }
} 