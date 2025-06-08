// Terminal functionality
function initializeTerminal() {
    const terminalContent = document.querySelector('.terminal-content');
    if (!terminalContent) return;

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
        
        const currentLine = terminalContent.querySelector('.terminal-line:last-child');
        const inputSpan = currentLine.querySelector('.terminal-input');
        
        if (handleSpecialKeys(e, inputSpan)) return;
        
        // Add character for regular keys
        inputSpan.textContent += e.key;
    }

    function handleSpecialKeys(e, inputSpan) {
        switch(e.key) {
            case 'Enter':
                handleEnterKey(inputSpan);
                return true;
            case 'Backspace':
                handleBackspaceKey(inputSpan);
                return true;
            case 'ArrowUp':
                handleArrowUpKey(inputSpan);
                return true;
            case 'ArrowDown':
                handleArrowDownKey(inputSpan);
                return true;
            case 'Tab':
                handleTabKey(inputSpan);
                return true;
            default:
                if (e.key.length > 1) return true;
                return false;
        }
    }

    function handleEnterKey(inputSpan) {
        const command = inputSpan.textContent;
        if (command.trim() !== '') {
            commandHistory.push(command);
            historyIndex = commandHistory.length;
        }
        processCommand(command);
    }

    function handleBackspaceKey(inputSpan) {
        const text = inputSpan.textContent;
        if (text.length > 0) {
            inputSpan.textContent = text.substring(0, text.length - 1);
        }
    }

    function handleArrowUpKey(inputSpan) {
        if (historyIndex > 0) {
            historyIndex--;
            inputSpan.textContent = commandHistory[historyIndex];
        }
    }

    function handleArrowDownKey(inputSpan) {
        if (historyIndex < commandHistory.length - 1) {
            historyIndex++;
            inputSpan.textContent = commandHistory[historyIndex];
        } else if (historyIndex === commandHistory.length - 1) {
            historyIndex = commandHistory.length;
            inputSpan.textContent = '';
        }
    }

    function handleTabKey(inputSpan) {
        const text = inputSpan.textContent.trim().toLowerCase();
        const commands = ['help', 'ls', 'pwd', 'echo', 'whoami', 'date', 'clear', 'uname', 'ping', 'history', 'cd', 'cat', 'ifconfig', 'netstat', 'top', 'ps'];
        const matchingCommands = commands.filter(cmd => cmd.startsWith(text));
        
        if (matchingCommands.length === 1) {
            inputSpan.textContent = matchingCommands[0] + ' ';
        } else if (matchingCommands.length > 1 && text !== '') {
            createOutputLine('Possible commands:');
            matchingCommands.forEach(cmd => {
                createOutputLine('  ' + cmd);
            });
        }
    }

    function processCommand(command) {
        const trimmedCmd = command.trim().toLowerCase();
        const commandArgs = trimmedCmd.split(' ');
        const baseCmd = commandArgs[0];
        
        const output = executeCommand(baseCmd, commandArgs, trimmedCmd);
        
        if (output) {
            if (output.includes('\n')) {
                output.trim().split('\n').forEach(line => createOutputLine(line));
            } else {
                createOutputLine(output);
            }
        }
        
        addNewLine();
        terminalContent.scrollTop = terminalContent.scrollHeight;
    }

    function executeCommand(baseCmd, commandArgs, trimmedCmd) {
        const commandHandlers = {
            'help': () => getHelpText(),
            'ls': () => handleLsCommand(commandArgs),
            'pwd': () => '/home/echo',
            'echo': () => commandArgs.slice(1).join(' '),
            'clear': () => { terminalContent.innerHTML = ''; addNewLine(); return null; },
            'whoami': () => 'echo',
            'date': () => new Date().toString(),
            'uname': () => 'ECHO OS v1.0.4 #1 SMP ECHO 5.15.8',
            'ping': () => handlePingCommand(commandArgs),
            'history': () => handleHistoryCommand(),
            'cd': () => `Changed directory to ${commandArgs[1] || '~'}`,
            'cat': () => handleCatCommand(commandArgs),
            'ifconfig': () => getIfconfigOutput(),
            'netstat': () => getNetstatOutput(),
            'top': () => getTopOutput(),
            'ps': () => getPsOutput()
        };

        if (trimmedCmd === '') return null;
        return commandHandlers[baseCmd] ? commandHandlers[baseCmd]() : `Command not found: ${baseCmd}`;
    }

    function getHelpText() {
        return `
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
    }

    function handleLsCommand(commandArgs) {
        if (commandArgs.length > 1 && commandArgs[1] === '-la') {
            return `
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
        }
        return `
Documents/  Downloads/  Pictures/  Music/  Videos/
config.ini  readme.md   .bashrc   .hidden
`;
    }

    function handlePingCommand(commandArgs) {
        const target = commandArgs[1] || 'example.com';
        return `
PING ${target} (93.184.216.34): 56 data bytes
64 bytes from 93.184.216.34: icmp_seq=0 ttl=56 time=11.632 ms
64 bytes from 93.184.216.34: icmp_seq=1 ttl=56 time=9.134 ms
64 bytes from 93.184.216.34: icmp_seq=2 ttl=56 time=9.456 ms
64 bytes from 93.184.216.34: icmp_seq=3 ttl=56 time=10.782 ms

--- ${target} ping statistics ---
4 packets transmitted, 4 packets received, 0.0% packet loss
round-trip min/avg/max/stddev = 9.134/10.251/11.632/1.043 ms
`;
    }

    function handleHistoryCommand() {
        return commandHistory.length === 0 ? 
            'No commands in history' : 
            commandHistory.map((cmd, i) => `${i + 1}  ${cmd}`).join('\n');
    }

    function handleCatCommand(commandArgs) {
        const file = commandArgs[1];
        if (!file) return 'Usage: cat [file]';
        
        const fileContents = {
            'readme.md': `
# Echo Project System

This is a demonstration of a terminal interface within a web application.
The terminal simulates basic Unix/Linux commands for educational purposes.

## Features
- Basic command line interface
- Network diagnostics tools
- System monitoring capabilities

For more information, contact the system administrator.
`,
            'config.ini': `
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
`
        };
        
        return fileContents[file] || `cat: ${file}: No such file or directory`;
    }

    function getIfconfigOutput() {
        return `
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
    }

    function getNetstatOutput() {
        return `
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
    }

    function getTopOutput() {
        return `
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
    }

    function getPsOutput() {
        return `
  PID TTY          TIME CMD
 2341 pts/0    00:00:00 bash
 2842 pts/0    00:00:00 ps
 1253 tty7     00:00:15 Xorg
 1386 tty7     00:01:23 gnome-shell
`;
    }

    function createOutputLine(text) {
        const outputElement = document.createElement('div');
        outputElement.className = 'terminal-line';
        outputElement.textContent = text;
        terminalContent.appendChild(outputElement);
    }

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

// Initialize terminal when DOM is loaded and we're on the terminal page
document.addEventListener('DOMContentLoaded', function() {
    if (window.location.pathname === '/Terminal') {
        initializeTerminal();
    }
}); 