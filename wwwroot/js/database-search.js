// Database Search functionality
document.addEventListener('DOMContentLoaded', function() {
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
}); 