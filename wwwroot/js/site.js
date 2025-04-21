// Site JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Highlight active page in sidebar based on current path
    const currentPath = window.location.pathname;
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    
    // First reset all dividers to their default colors
    document.querySelectorAll('.dividers .line-1, .dividers .line-2, .dividers .line-3, .dividers .line-4, .dividers .line-5, .dividers .line-6, .dividers .line-7').forEach(line => {
        if (line.classList.contains('line-1')) {
            line.style.backgroundColor = '#5E5E5E'; //top line always grey! unless active
        } else if (line.classList.contains('line-2')) {
            line.style.backgroundColor = '#5E5E5E';
        } else {
            line.style.backgroundColor = '#5E5E5E';
        }
    });
    
    // Map to connect pages with their corresponding divider lines
    const pageToDividerMap = {
        '/': '.dividers .line-3',
        '/Index': ['.dividers .line-2', '.dividers .line-3'],
        '/Terminal': ['.dividers .line-3', '.dividers .line-4'],
        '/RemoteAccess': ['.dividers .line-4', '.dividers .line-5'],
        '/Documentation': ['.dividers .line-5', '.dividers .line-6'],
        '/Network': ['.dividers .line-6', '.dividers .line-7']
    };
    
    // Highlight only the active sidebar link and its corresponding divider
    sidebarLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPath || 
            (currentPath === '/' && href === '/Index') || 
            (href === '/' && currentPath === '/Index')) {
            
            // Add active class to the link
            link.classList.add('active');
            
            // Highlight the correct divider based on the active page
            const dividerSelector = pageToDividerMap[href];
            if (dividerSelector) {
                if (Array.isArray(dividerSelector)) {
                    // If it's an array, handle each selector
                    dividerSelector.forEach(selector => {
                        const divider = document.querySelector(selector);
                        if (divider) {
                            divider.style.backgroundColor = '#FFFFFF';
                        }
                    });
                } else {
                    // Handle single string selector
                    const divider = document.querySelector(dividerSelector);
                    if (divider) {
                        divider.style.backgroundColor = '#FFFFFF';
                    }
                }
            }
            
            // If this is the top level page, also highlight the top line
            if (href === '/' || href === '/Index') {
                const topLine = document.querySelector('.dividers .line-1');
                if (topLine) {
                    topLine.style.backgroundColor = '#FFFFFF';
                }
            }
        }
    });

    // Search functionality
    const searchBtn = document.getElementById('search-btn');
    const searchQuery = document.getElementById('search-query');
    const searchResults = document.getElementById('search-results');

    if (searchBtn && searchQuery && searchResults) {
        searchBtn.addEventListener('click', function() {
            performSearch();
        });

        // Also trigger search on Enter key press
        searchQuery.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                performSearch();
            }
        });

        function performSearch() {
            const query = searchQuery.value.trim();
            
            if (!query) {
                alert('Please enter a search query');
                return;
            }
            
            // In a real application, this would make an API call
            // For demo purposes, we'll just display the pre-defined results
            searchResults.style.display = 'block';
            
            // Scroll to results
            searchResults.scrollIntoView({ behavior: 'smooth' });
        }
    }

    // Add click event for other search buttons
    const otherSearchButtons = document.querySelectorAll('.search-button button:not(#search-btn)');
    otherSearchButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Handle remote access connection
            if (window.location.pathname === '/RemoteAccess') {
                handleRemoteConnection();
            }
            // Handle network tool buttons
            else if (window.location.pathname === '/Network') {
                // Button ID is already handled by the network buttons selector
                if (!this.id) {
                    const buttonText = this.textContent.trim();
                    handleNetworkTool(buttonText);
                }
            }
        });
    });

    // Network tool buttons
    const networkButtons = document.querySelectorAll('.tool-buttons .search-button button');
    networkButtons.forEach(button => {
        button.addEventListener('click', function() {
            const buttonText = this.textContent.trim().replace('_', '');
            handleNetworkTool(buttonText);
        });
    });

    // Handle network tools
    function handleNetworkTool(tool) {
        let message = '';
        
        switch(tool) {
            case 'Ping':
                message = 'Enter IP address or hostname to ping:';
                break;
            case 'Traceroute':
                message = 'Enter IP address or hostname to trace:';
                break;
            case 'DNS Lookup':
                message = 'Enter domain to lookup:';
                break;
            case 'Port Scan':
                message = 'Enter IP address to scan:';
                break;
            default:
                message = 'Select a tool:';
        }
        
        const target = prompt(message);
        if (target) {
            alert(`${tool} to ${target} initiated (demo only)`);
        }
    }

    // Handle remote connection
    function handleRemoteConnection() {
        const hostname = document.getElementById('hostname').value;
        const port = document.getElementById('port').value;
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        
        if (!hostname || !username || !password) {
            alert('Please fill in all required fields');
            return;
        }
        
        alert(`Connecting to ${hostname}:${port} as ${username} (demo only)`);
    }

    // Terminal functionality - simulate typing
    const terminalContent = document.querySelector('.terminal-content');
    if (terminalContent) {
        // Command history
        const commandHistory = [];
        let historyIndex = -1;
        
        // Handle initial setup - ensure only one blinking underscore when page loads
        const initTerminal = () => {
            const allUnderscores = terminalContent.querySelectorAll('.underscore');
            
            // Remove blink class from all underscores except the last one
            if (allUnderscores.length > 0) {
                for (let i = 0; i < allUnderscores.length - 1; i++) {
                    allUnderscores[i].classList.remove('blink-underscore');
                }
            }
        };
        
        // Run initial setup
        initTerminal();
        
        // Add the first command line if not already present
        if (!terminalContent.querySelector('.terminal-line:last-child .terminal-input')) {
            addNewLine();
        }
        
        // Focus on terminal when clicked
        const terminalWindow = document.querySelector('.terminal-window');
        terminalContent.addEventListener('click', function() {
            terminalWindow.classList.add('focused');
            document.addEventListener('keydown', handleTerminalInput);
        });
        
        // Remove focus when clicking outside
        document.addEventListener('click', function(e) {
            if (!terminalWindow.contains(e.target)) {
                terminalWindow.classList.remove('focused');
            }
        });
        
        // Handle keypress in terminal
        function handleTerminalInput(e) {
            if (!terminalWindow.classList.contains('focused')) return;
            
            // Get current line and its parts
            const currentLine = terminalContent.querySelector('.terminal-line:last-child');
            const inputSpan = currentLine.querySelector('.terminal-input');
            
            // Handle special keys
            if (e.key === 'Enter') {
                e.preventDefault();
                // Process command
                const command = inputSpan.textContent;
                
                // Add to history if not empty
                if (command.trim() !== '') {
                    commandHistory.push(command);
                    historyIndex = commandHistory.length;
                }
                
                processCommand(command);
                return;
            } else if (e.key === 'Backspace') {
                e.preventDefault();
                // Remove last character
                const text = inputSpan.textContent;
                if (text.length > 0) {
                    inputSpan.textContent = text.substring(0, text.length - 1);
                }
                return;
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                // Navigate up in history
                if (historyIndex > 0) {
                    historyIndex--;
                    inputSpan.textContent = commandHistory[historyIndex];
                }
                return;
            } else if (e.key === 'ArrowDown') {
                e.preventDefault();
                // Navigate down in history
                if (historyIndex < commandHistory.length - 1) {
                    historyIndex++;
                    inputSpan.textContent = commandHistory[historyIndex];
                } else if (historyIndex === commandHistory.length - 1) {
                    historyIndex = commandHistory.length;
                    inputSpan.textContent = '';
                }
                return;
            } else if (e.key === 'Tab') {
                e.preventDefault();
                // Auto-complete command
                const text = inputSpan.textContent.trim().toLowerCase();
                const commands = ['help', 'ls', 'pwd', 'echo', 'whoami', 'date', 'clear', 'uname', 'ping', 'history', 'cd', 'cat', 'ifconfig', 'netstat', 'top', 'ps'];
                
                // Find commands that start with the current text
                const matchingCommands = commands.filter(cmd => cmd.startsWith(text));
                
                if (matchingCommands.length === 1) {
                    // Exact match, complete the command
                    inputSpan.textContent = matchingCommands[0] + ' ';
                } else if (matchingCommands.length > 1 && text !== '') {
                    // Multiple matches, show them
                    createOutputLine('Possible commands:');
                    matchingCommands.forEach(cmd => {
                        createOutputLine('  ' + cmd);
                    });
                }
                return;
            } else if (e.key.length > 1) {
                // Ignore special keys like Shift, Ctrl, Alt, etc.
                return;
            }
            
            // Add character
            inputSpan.textContent += e.key;
        }
        
        // Process terminal command
        function processCommand(command) {
            let output = '';
            const trimmedCmd = command.trim().toLowerCase();
            const commandArgs = trimmedCmd.split(' ');
            const baseCmd = commandArgs[0];
            
            // Simple command processing
            if (baseCmd === 'help') {
                output = `
Available commands:
  help        - Display this help message
  ls          - List directory contents
  pwd         - Print working directory
  echo [text] - Display a line of text
  whoami      - Display current user
  date        - Display current date and time
  clear       - Clear the terminal screen
  uname       - Display system information
  ping        - Network connectivity test simulation
  history     - Command history
  cd          - Change directory (simulated)
  cat         - Show file contents (simulated)
  ifconfig    - Display network interfaces
  netstat     - Network statistics
  top         - Display system processes
  ps          - List running processes
`;
            } else if (baseCmd === 'ls') {
                // Check for arguments
                if (commandArgs.length > 1 && commandArgs[1] === '-la') {
                    output = `
total 68
drwxr-xr-x 14 echo echo  4096 Jun 12 14:22 .
drwxr-xr-x  3 root root  4096 May  8 09:15 ..
-rw-------  1 echo echo  5982 Jun 12 13:55 .bash_history
-rw-r--r--  1 echo echo   220 May  8 09:15 .bash_logout
-rw-r--r--  1 echo echo  3771 May  8 09:15 .bashrc
drwxr-xr-x  8 echo echo  4096 Jun 10 16:33 Documents
drwxr-xr-x  2 echo echo  4096 Jun  8 22:15 Downloads
-rw-r--r--  1 echo echo   807 May  8 09:15 .profile
drwxr-xr-x  2 echo echo  4096 May 19 12:07 Pictures
drwxr-xr-x  2 echo echo  4096 May  8 09:42 Music
drwxr-xr-x  2 echo echo  4096 May  8 09:42 Videos
-rw-r--r--  1 echo echo  2345 Jun  9 18:25 config.ini
-rw-r--r--  1 echo echo  1289 Jun  8 11:13 readme.md
`;
                } else {
                    output = `
Documents/  Downloads/  Pictures/  Music/  Videos/
config.ini  readme.md   .bashrc   .hidden
`;
                }
            } else if (baseCmd === 'pwd') {
                output = '/home/echo';
            } else if (baseCmd === 'echo') {
                output = commandArgs.slice(1).join(' ');
            } else if (baseCmd === 'clear') {
                terminalContent.innerHTML = '';
                addNewLine();
                return;
            } else if (baseCmd === 'whoami') {
                output = 'echo';
            } else if (baseCmd === 'date') {
                output = new Date().toString();
            } else if (baseCmd === 'uname' || trimmedCmd === 'uname -a') {
                output = 'ECHO OS v1.0.4 #1 SMP ECHO 5.15.8';
            } else if (baseCmd === 'ping') {
                const target = commandArgs[1] || 'example.com';
                output = `
PING ${target} (93.184.216.34): 56 data bytes
64 bytes from 93.184.216.34: icmp_seq=0 ttl=56 time=11.632 ms
64 bytes from 93.184.216.34: icmp_seq=1 ttl=56 time=9.134 ms
64 bytes from 93.184.216.34: icmp_seq=2 ttl=56 time=9.456 ms
64 bytes from 93.184.216.34: icmp_seq=3 ttl=56 time=10.782 ms

--- ${target} ping statistics ---
4 packets transmitted, 4 packets received, 0.0% packet loss
round-trip min/avg/max/stddev = 9.134/10.251/11.632/1.043 ms
`;
            } else if (baseCmd === 'history') {
                if (commandHistory.length === 0) {
                    output = 'No commands in history';
                } else {
                    output = commandHistory.map((cmd, i) => `${i + 1}  ${cmd}`).join('\n');
                }
            } else if (baseCmd === 'cd') {
                const dir = commandArgs[1] || '~';
                output = `Changed directory to ${dir}`;
            } else if (baseCmd === 'cat') {
                const file = commandArgs[1];
                if (!file) {
                    output = 'Usage: cat [file]';
                } else if (file === 'readme.md') {
                    output = `
# Echo Project System

This is a demonstration of a terminal interface within a web application.
The terminal simulates basic Unix/Linux commands for educational purposes.

## Features
- Basic command line interface
- Network diagnostics tools
- System monitoring capabilities

For more information, contact the system administrator.
`;
                } else if (file === 'config.ini') {
                    output = `
[System]
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
PasswordPolicy=strong
`;
                } else {
                    output = `cat: ${file}: No such file or directory`;
                }
            } else if (baseCmd === 'ifconfig') {
                output = `
eth0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 192.168.1.100  netmask 255.255.255.0  broadcast 192.168.1.255
        inet6 fe80::215:5dff:fe36:7a23  prefixlen 64  scopeid 0x20<link>
        ether 00:15:5d:36:7a:23  txqueuelen 1000  (Ethernet)
        RX packets 2845761  bytes 3317225169 (3.0 GiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 1353850  bytes 143308763 (136.6 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

lo: flags=73<UP,LOOPBACK,RUNNING>  mtu 65536
        inet 127.0.0.1  netmask 255.0.0.0
        inet6 ::1  prefixlen 128  scopeid 0x10<host>
        loop  txqueuelen 1000  (Local Loopback)
        RX packets 5445  bytes 4032956 (3.8 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 5445  bytes 4032956 (3.8 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0
`;
            } else if (baseCmd === 'netstat') {
                output = `
Active Internet connections (only servers)
Proto Recv-Q Send-Q Local Address           Foreign Address         State      
tcp        0      0 0.0.0.0:22              0.0.0.0:*               LISTEN     
tcp        0      0 127.0.0.1:3306          0.0.0.0:*               LISTEN     
tcp        0      0 0.0.0.0:80              0.0.0.0:*               LISTEN     
tcp        0      0 0.0.0.0:443             0.0.0.0:*               LISTEN     
tcp6       0      0 :::22                   :::*                    LISTEN     
tcp6       0      0 :::80                   :::*                    LISTEN     
udp        0      0 0.0.0.0:68              0.0.0.0:*                          
`;
            } else if (baseCmd === 'top') {
                output = `
top - 15:42:23 up 10 days,  4:40,  2 users,  load average: 0.08, 0.05, 0.01
Tasks: 108 total,   1 running, 107 sleeping,   0 stopped,   0 zombie
%Cpu(s):  2.1 us,  0.4 sy,  0.0 ni, 97.5 id,  0.0 wa,  0.0 hi,  0.0 si,  0.0 st
MiB Mem :   7977.4 total,   5162.3 free,   1297.2 used,   1517.9 buff/cache
MiB Swap:   2048.0 total,   2047.8 free,      0.2 used.   6374.8 avail Mem 

    PID USER      PR  NI    VIRT    RES    SHR S  %CPU  %MEM     TIME+ COMMAND
   1253 echo      20   0  707076  63292  42996 S   0.3   0.8   0:15.42 Xorg
   1386 echo      20   0 3495796 215456 108304 S   0.3   2.6   1:23.18 gnome-shell
      1 root      20   0  166944  11996   8144 S   0.0   0.1   0:03.42 systemd
      2 root      20   0       0      0      0 S   0.0   0.0   0:00.00 kthreadd
     11 root       0 -20       0      0      0 I   0.0   0.0   0:00.00 rcu_tasks_tr+
`;
            } else if (baseCmd === 'ps') {
                output = `
  PID TTY          TIME CMD
 2341 pts/0    00:00:00 bash
 2842 pts/0    00:00:00 ps
 1253 tty7     00:00:15 Xorg
 1386 tty7     00:01:23 gnome-shell
`;
            } else if (trimmedCmd === '') {
                // Empty command, just add a new line
            } else {
                output = `Command not found: ${baseCmd}`;
            }
            
            // Add output (handle multi-line output with proper formatting)
            if (output) {
                if (output.includes('\n')) {
                    const lines = output.trim().split('\n');
                    lines.forEach(line => {
                        createOutputLine(line);
                    });
                } else {
                    createOutputLine(output);
                }
            }
            
            // Add new line
            addNewLine();
            
            // Scroll to bottom
            terminalContent.scrollTop = terminalContent.scrollHeight;
        }
        
        // Create output line
        function createOutputLine(text) {
            const outputElement = document.createElement('div');
            outputElement.className = 'terminal-line';
            outputElement.textContent = text;
            terminalContent.appendChild(outputElement);
        }
        
        // Add a new command line
        function addNewLine() {
            // Remove blinking effect from all previous underscores
            const previousUnderscores = terminalContent.querySelectorAll('.blink-underscore');
            previousUnderscores.forEach(underscoreElem => {
                underscoreElem.classList.remove('blink-underscore');
            });
            
            const newLine = document.createElement('div');
            newLine.className = 'terminal-line';
            
            // Create parts of the line
            const promptText = document.createTextNode('echo@server:~ $ ');
            const inputSpan = document.createElement('span');
            inputSpan.className = 'terminal-input';
            
            const underscoreSpan = document.createElement('span');
            underscoreSpan.className = 'underscore blink-underscore';
            underscoreSpan.textContent = '_';
            
            // Assemble the line
            newLine.appendChild(promptText);
            newLine.appendChild(inputSpan);
            newLine.appendChild(underscoreSpan);
            
            terminalContent.appendChild(newLine);
            
            // Scroll to bottom
            terminalContent.scrollTop = terminalContent.scrollHeight;
        }
    }

    // Add hover effects for sidebar items
    const sidebarItems = document.querySelectorAll('.sql-query, .ssh-terminal, .remote-access, .documentation, .network');
    sidebarItems.forEach(item => {
        // Skip the terminal item to prevent any hover effects
        if (item.classList.contains('ssh-terminal') && item.closest('.sidebar-link').getAttribute('href') === '/Terminal') {
            return;
        }
        
        // Only add hover effect to non-active items
        if (!item.closest('.sidebar-link').classList.contains('active')) {
            item.addEventListener('mouseenter', function() {
                if (!this.closest('.sidebar-link').classList.contains('active')) {
                    this.style.backgroundColor = '#2A2A2A';
                }
            });
            
            item.addEventListener('mouseleave', function() {
                if (!this.closest('.sidebar-link').classList.contains('active')) {
                    this.style.backgroundColor = '';
                }
            });
        }
    });
}); 