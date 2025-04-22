using System;
using System.Collections.Generic;
using System.IO;
using Project_Echo.Models;

namespace Project_Echo.Services
{
    public class TerminalService
    {
        private readonly Dictionary<string, Func<TerminalSession, string[], string>> _commands;
        private const int MaxHistoryItems = 100;

        public TerminalService()
        {
            _commands = new Dictionary<string, Func<TerminalSession, string[], string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["help"] = (session, args) => GetHelpText(),
                ["clear"] = (session, args) => string.Empty, // Clear is handled differently in the view
                ["echo"] = (session, args) => string.Join(" ", args, 1, Math.Max(0, args.Length - 1)),
                ["pwd"] = (session, args) => session.CurrentDirectory,
                ["ls"] = (session, args) => ListDirectory(session, args),
                ["cd"] = (session, args) => ChangeDirectory(session, args),
                ["whoami"] = (session, args) => "echo",
                ["date"] = (session, args) => DateTime.Now.ToString(),
                ["history"] = (session, args) => GetHistory(session),
                ["cat"] = (session, args) => ShowFileContents(session, args)
            };
        }

        public TerminalResponse ProcessCommand(TerminalSession session, string commandText)
        {
            var response = new TerminalResponse();
            
            if (string.IsNullOrWhiteSpace(commandText))
            {
                return response;
            }

            // Split the command into parts
            var commandParts = commandText.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (commandParts.Length == 0)
            {
                return response;
            }

            var command = commandParts[0].ToLowerInvariant();
            
            // Process the command
            string output;
            if (_commands.TryGetValue(command, out var handler))
            {
                output = handler(session, commandParts);
            }
            else
            {
                output = $"Command not found: {command}";
            }

            // Add to history
            session.History.Add(new TerminalHistoryItem
            {
                Command = commandText,
                Output = output,
                Timestamp = DateTime.Now
            });
            
            // Trim history if needed
            if (session.History.Count > MaxHistoryItems)
            {
                session.History.RemoveAt(0);
            }
            
            // Set response output
            response.Output = output;
            
            return response;
        }

        private string GetHelpText()
        {
            return @"Available commands:
  help        - Display this help message
  clear       - Clear the terminal screen
  echo [text] - Display a line of text
  pwd         - Print working directory
  ls          - List directory contents
  cd [dir]    - Change directory
  whoami      - Display current user
  date        - Display current date and time
  history     - Show command history
  cat [file]  - Show file contents";
        }

        private string ListDirectory(TerminalSession session, string[] args)
        {
            // In a real implementation, this would list actual directory contents
            return @"Documents/  Downloads/  Pictures/  Music/  Videos/
config.ini  readme.md   .bashrc   .hidden";
        }

        private string ChangeDirectory(TerminalSession session, string[] args)
        {
            if (args.Length < 2)
            {
                session.CurrentDirectory = "/home/echo";
                return "Changed to home directory";
            }

            string newDir = args[1];
            
            // Handle special cases
            if (newDir == "~" || newDir == "")
            {
                session.CurrentDirectory = "/home/echo";
                return "Changed to home directory";
            }
            
            if (newDir == "..")
            {
                var parts = session.CurrentDirectory.TrimEnd('/').Split('/');
                if (parts.Length > 2)
                {
                    session.CurrentDirectory = string.Join("/", parts, 0, parts.Length - 1);
                    return $"Changed directory to {session.CurrentDirectory}";
                }
                else
                {
                    return "Already at root level";
                }
            }
            
            // Simulate directory change (in a real implementation, would validate the directory exists)
            if (newDir.StartsWith("/"))
            {
                session.CurrentDirectory = newDir;
            }
            else
            {
                session.CurrentDirectory = session.CurrentDirectory.TrimEnd('/') + "/" + newDir;
            }
            
            return $"Changed directory to {session.CurrentDirectory}";
        }

        private string GetHistory(TerminalSession session)
        {
            if (session.History.Count == 0)
            {
                return "No commands in history";
            }
            
            var lines = new List<string>();
            for (int i = 0; i < session.History.Count; i++)
            {
                lines.Add($"{i + 1}  {session.History[i].Command}");
            }
            
            return string.Join(Environment.NewLine, lines);
        }

        private string ShowFileContents(TerminalSession session, string[] args)
        {
            if (args.Length < 2)
            {
                return "Usage: cat [file]";
            }
            
            string filename = args[1];
            
            // In a real implementation, this would read actual files
            return filename switch
            {
                "readme.md" => @"# Echo Project System

This is a demonstration of a terminal interface within a web application.
The terminal simulates basic Unix/Linux commands for educational purposes.

## Features
- Basic command line interface
- Network diagnostics tools
- System monitoring capabilities

For more information, contact the system administrator.",
                
                "config.ini" => @"[System]
Version=1.0.4
LastUpdate=2023-06-09
Hostname=echo-server

[Network]
IP=192.168.1.100
Gateway=192.168.1.1
DNS=8.8.8.8,1.1.1.1

[Security]
FirewallEnabled=true
AutomaticUpdates=true
PasswordPolicy=strong",
                
                _ => $"cat: {filename}: No such file or directory"
            };
        }
    }
} 