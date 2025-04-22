using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class TerminalModel : PageModel
    {
        public readonly ILogger<TerminalModel> _logger;

        public TerminalModel(ILogger<TerminalModel> logger)
        {
            _logger = logger;
        }
        
    }
} 