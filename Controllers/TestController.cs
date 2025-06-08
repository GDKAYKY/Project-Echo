using Microsoft.AspNetCore.Mvc;

namespace Project_Echo.Controllers
{
    public class TestController : Controller
    {
        // GET: /Test/
        public IActionResult Index()
        {
            return View();
        }
    }
} 