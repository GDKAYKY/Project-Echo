// Site JavaScript
document.addEventListener('DOMContentLoaded', function() {
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