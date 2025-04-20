// Site JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Add click event for search button
    const searchButton = document.querySelector('.search-button button');
    if (searchButton) {
        searchButton.addEventListener('click', function() {
            // Check if we're on the index page (database search)
            if (window.location.pathname === '/' || window.location.pathname === '/index' || window.location.pathname === '/Index') {
                window.location.href = '/Features';
            } 
            // Handle login form submission
            else if (window.location.pathname === '/Login') {
                handleFormSubmission('login');
            }
            // Handle contact form submission
            else if (window.location.pathname === '/Contact') {
                handleFormSubmission('contact');
            }
        });
    }

    // Function to handle form submissions
    function handleFormSubmission(formType) {
        if (formType === 'login') {
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            
            if (!username || !password) {
                alert('Please fill in all fields');
                return;
            }
            
            // In a real application, this would make an API call
            alert('Login successful (demo only)');
            window.location.href = '/';
        } 
        else if (formType === 'contact') {
            const name = document.getElementById('name').value;
            const email = document.getElementById('email').value;
            const message = document.getElementById('message').value;
            
            if (!name || !email || !message) {
                alert('Please fill in all fields');
                return;
            }
            
            // In a real application, this would make an API call
            alert('Message sent successfully (demo only)');
            document.getElementById('name').value = '';
            document.getElementById('email').value = '';
            document.getElementById('message').value = '';
        }
    }

    // Add hover effects for sidebar items
    const sidebarItems = document.querySelectorAll('.sql-query, .ssh-terminal, .remote-access, .documentation, .network');
    sidebarItems.forEach(item => {
        item.addEventListener('mouseenter', function() {
            this.style.backgroundColor = '#2A2A2A';
        });
        
        item.addEventListener('mouseleave', function() {
            this.style.backgroundColor = '';
        });
    });
}); 