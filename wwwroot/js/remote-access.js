// Remote Access functionality
document.addEventListener('DOMContentLoaded', function() {
    // Add click event for remote access buttons
    const remoteAccessButtons = document.querySelectorAll('.search-button button');
    remoteAccessButtons.forEach(button => {
        button.addEventListener('click', function() {
            if (window.location.pathname === '/RemoteAccess') {
                handleRemoteConnection();
            }
        });
    });

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
}); 