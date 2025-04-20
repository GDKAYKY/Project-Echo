using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class TerminalModel : PageModel
    {
        private readonly ILogger<TerminalModel> _logger;

        public TerminalModel(ILogger<TerminalModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
} 