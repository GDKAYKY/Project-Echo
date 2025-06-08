using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Project_Echo.Controllers
{
    public class ActivePageController : Controller
    {
        // In a real application, this mapping would come from a dedicated document service or database.
        private static readonly Dictionary<string, string> _documentMap = new Dictionary<string, string>
        {
            { "/documentation/sampledocument", "Sample Active Page" },
            { "/documentation/anotherdocument", "Another Great Document" },
            // Add more document paths and titles as needed
        };

        // This action could be used to retrieve information about the currently active page
        // or to provide data necessary for setting the active state in client-side scripts.
        public IActionResult GetActivePageInfo()
        {
            var currentPath = HttpContext.Request.Path.Value?.ToLowerInvariant();
            string documentTitle = "Unknown Document";

            if (currentPath != null && _documentMap.TryGetValue(currentPath, out var title))
            {
                documentTitle = title;
            }

            var activePageData = new
            {
                title = documentTitle,
                path = currentPath ?? "/", // Ensure path is not null
            };

            return Json(activePageData);
        }
    }
} 