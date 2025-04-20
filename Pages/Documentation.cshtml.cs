using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class DocumentationModel : PageModel
    {
        private readonly ILogger<DocumentationModel> _logger;

        public DocumentationModel(ILogger<DocumentationModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
} 