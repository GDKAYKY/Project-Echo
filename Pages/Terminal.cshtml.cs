using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Echo.Models;
using Project_Echo.Services;

namespace Project_Echo.Pages
{
    public class TerminalModel(TerminalService terminalService) : PageModel
    {
        private readonly TerminalService _terminalService = terminalService;
        
        public TerminalViewModel TerminalVM { get; private set; } = new();
        
        [BindProperty]
        public string CommandInput { get; set; } = "";
        
        public void OnGet()
        {
            // Initialize or retrieve the session
            var sessionId = HttpContext.Session.GetString("TerminalSessionId");
            
            if (string.IsNullOrEmpty(sessionId))
            {
                // New session
                TerminalVM.Session = new TerminalSession();
                HttpContext.Session.SetString("TerminalSessionId", TerminalVM.Session.SessionId);
                
                // Add welcome message
                TerminalVM.Session.History.Add(new TerminalHistoryItem
                {
                    Command = "",
                    Output = $"Welcome to ECHO Terminal. Last login: {DateTime.Now:ddd MMM d HH:mm:ss}\nType 'help' for available commands.",
                    Timestamp = DateTime.Now
                });
            }
            else
            {
                // Try to retrieve the existing session from TempData
                var sessionJson = TempData["TerminalSession"] as string;
                if (!string.IsNullOrEmpty(sessionJson))
                {
                    try 
                    {
                        TerminalVM.Session = System.Text.Json.JsonSerializer.Deserialize<TerminalSession>(sessionJson)
                            ?? new TerminalSession();
                    }
                    catch
                    {
                        // If deserialization fails, create a new session
                        TerminalVM.Session = new TerminalSession();
                    }
                }
                else
                {
                    TerminalVM.Session = new TerminalSession();
                }
            }
        }
        
        public IActionResult OnPost()
        {
            // Process the command
            if (!string.IsNullOrWhiteSpace(CommandInput))
            {
                // Initialize session
                var sessionJson = TempData["TerminalSession"] as string;
                TerminalVM.Session = !string.IsNullOrEmpty(sessionJson)
                    ? System.Text.Json.JsonSerializer.Deserialize<TerminalSession>(sessionJson) ?? new TerminalSession()
                    : new TerminalSession();
                
                // Clear the terminal if requested
                if (CommandInput.Trim().Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    TerminalVM.Session.History.Clear();
                }
                else
                {
                    // Process command
                    _terminalService.ProcessCommand(TerminalVM.Session, CommandInput);
                }
                
                // Save session for next request
                TempData["TerminalSession"] = System.Text.Json.JsonSerializer.Serialize(TerminalVM.Session);
                
                // Clear command input
                CommandInput = "";
            }
            
            return Page();
        }
    }
} 