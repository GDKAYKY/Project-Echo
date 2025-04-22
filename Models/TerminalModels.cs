using System;
using System.Collections.Generic;

namespace Project_Echo.Models
{
    public class TerminalCommand
    {
        public string Command { get; set; } = string.Empty;
    }
    
    public class TerminalResponse
    {
        public string Output { get; set; } = string.Empty;
        public string Prompt { get; set; } = "echo@server:~ $";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
    
    public class TerminalSession
    {
        public List<TerminalHistoryItem> History { get; set; } = new List<TerminalHistoryItem>();
        public string CurrentDirectory { get; set; } = "/home/echo";
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
    }
    
    public class TerminalHistoryItem
    {
        public string Command { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
    
    public class TerminalViewModel
    {
        public TerminalSession Session { get; set; } = new TerminalSession();
        public string CurrentCommand { get; set; } = string.Empty;
    }
} 