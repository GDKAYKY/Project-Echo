using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class NetworkModel : PageModel
    {
        private readonly ILogger<NetworkModel> _logger;

        private NetworkModel(ILogger<NetworkModel> logger)
        {
            _logger = logger;
        }
        
    }
} 