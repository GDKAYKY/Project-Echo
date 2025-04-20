using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class RemoteAccessModel : PageModel
    {
        private readonly ILogger<RemoteAccessModel> _logger;

        public RemoteAccessModel(ILogger<RemoteAccessModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
} 