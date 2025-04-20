// Site JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Highlight active page in sidebar based on current path
    const currentPath = window.location.pathname;
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    
    sidebarLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPath || 
            (currentPath === '/' && href === '/Index') || 
            (href === '/' && currentPath === '/Index')) {
            link.classList.add('active');
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
        // Focus on terminal when clicked
        terminalContent.addEventListener('click', function() {
            this.classList.add('focused');
            document.addEventListener('keydown', handleTerminalInput);
        });
        
        // Handle keypress in terminal
        function handleTerminalInput(e) {
            if (!terminalContent.classList.contains('focused')) return;
            
            // Get current line and its parts
            const currentLine = terminalContent.querySelector('.terminal-line:last-child');
            const inputSpan = currentLine.querySelector('.terminal-input');
            
            // Handle special keys
            if (e.key === 'Enter') {
                e.preventDefault();
                // Process command
                const command = inputSpan.textContent;
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
            
            // Simple command processing
            if (command.toLowerCase().includes('help')) {
                output = 'Available commands: help, ls, pwd, echo, clear';
            } else if (command.toLowerCase().includes('ls')) {
                output = 'Documents  Downloads  Pictures  Music  Videos';
            } else if (command.toLowerCase().includes('pwd')) {
                output = '/home/echo';
            } else if (command.toLowerCase().startsWith('echo ')) {
                output = command.substring(5);
            } else if (command.toLowerCase() === 'clear') {
                terminalContent.innerHTML = '';
                addNewLine();
                return;
            } else if (command.trim() === '') {
                // Empty command, just add a new line
            } else {
                output = `Command not found: ${command}`;
            }
            
            // Add output
            if (output) {
                const outputElement = document.createElement('div');
                outputElement.textContent = output;
                terminalContent.appendChild(outputElement);
            }
            
            // Add new line
            addNewLine();
        }
        
        // Add a new command line
        function addNewLine() {
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
    });
}); 