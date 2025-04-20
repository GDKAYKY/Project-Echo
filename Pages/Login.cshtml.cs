using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Echo.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
} 