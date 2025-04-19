using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EchoSearch.Models;
using EchoSearch.Services;

namespace EchoSearch.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISearchService _searchService;

    public HomeController(ILogger<HomeController> logger, ISearchService searchService)
    {
        _logger = logger;
        _searchService = searchService;
    }

    public IActionResult Index()
    {
        // Add a header to ensure no caching of CSS
        Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        Response.Headers.Add("Pragma", "no-cache");
        Response.Headers.Add("Expires", "0");
        
        return View();
    }

    [HttpGet]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Json(new { results = Array.Empty<SearchResult>() });
        }

        var results = _searchService.Search(query);
        return Json(new { results });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
