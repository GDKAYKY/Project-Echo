// Network functionality
document.addEventListener('DOMContentLoaded', function() {
    // Add click event for network tool buttons
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
}); 