using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Echo.Models;
using Project_Echo.Services;
using System.ComponentModel.DataAnnotations;

namespace Project_Echo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IDatabaseService databaseService, ILogger<IndexModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        [BindProperty]
        public DatabaseViewModel ViewModel { get; set; } = new DatabaseViewModel();

        public async Task OnGetAsync()
        {
            // Load existing database connections
            ViewModel.Connections = await _databaseService.GetConnectionsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ViewModel.UploadModel.DatabaseFile == null && string.IsNullOrEmpty(ViewModel.UploadModel.ConnectionString))
            {
                ModelState.AddModelError("", "Either a database file or connection string must be provided.");
                return Page();
            }

            try
            {
                // Upload the database
                var connection = await _databaseService.UploadDatabaseAsync(ViewModel.UploadModel);

                // Test the connection
                var isConnected = await _databaseService.TestConnectionAsync(connection);
                if (!isConnected)
                {
                    ModelState.AddModelError("", "Could not connect to the database. Please check your connection details.");
                    return Page();
                }

                // Reload connections
                ViewModel.Connections = await _databaseService.GetConnectionsAsync();

                // Show success message
                TempData["SuccessMessage"] = "Database uploaded successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading database");
                ModelState.AddModelError("", $"Error uploading database: {ex.Message}");
                return Page();
            }
        }
    }
} 