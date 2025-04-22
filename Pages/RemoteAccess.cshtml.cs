using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class RemoteAccessModel : PageModel
    {
        private readonly ILogger<RemoteAccessModel> _logger;

        private RemoteAccessModel(ILogger<RemoteAccessModel> logger)
        {
            _logger = logger;
        }
        
    }
} 