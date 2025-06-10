// Database Search functionality
document.addEventListener('DOMContentLoaded', function() {
    // Search functionality
    const searchBtn = document.getElementById('search-btn');
    const searchQuery = document.getElementById('search-query');
    const searchResults = document.getElementById('search-results');
    const databaseSelect = document.getElementById('database-select');
    const openUploadPopupBtn = document.getElementById('open-upload-popup');
    const uploadPopup = document.getElementById('upload-popup');
    const closePopupBtn = uploadPopup?.querySelector('.close-popup');

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

        databaseSelect.addEventListener('change', function() {
            const databaseId = databaseSelect?.value;
            if (databaseId) {
                fetchAndDisplayTables(databaseId);
            }
        });

        // Handle popup opening and closing
        if (openUploadPopupBtn && uploadPopup && closePopupBtn) {
            openUploadPopupBtn.addEventListener('click', function() {
                uploadPopup.style.display = 'flex'; // Show the popup
            });

            closePopupBtn.addEventListener('click', function() {
                uploadPopup.style.display = 'none'; // Hide the popup
            });

            // Close popup when clicking outside the content
            window.addEventListener('click', function(event) {
                if (event.target === uploadPopup) {
                    uploadPopup.style.display = 'none';
                }
            });
        }

        async function performSearch() {
            const query = searchQuery.value.trim();
            const databaseId = databaseSelect?.value;
            
            if (!query) {
                alert('Please enter a search query');
                return;
            }

            if (!databaseId) {
                alert('Please select a database');
                return;
            }
            
            try {
                const response = await fetch('/api/database/query', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        connectionId: databaseId,
                        query: query
                    })
                });

                if (!response.ok) {
                    throw new Error('Query failed');
                }

                const data = await response.json();
                displayResults(data);
            } catch (error) {
                console.error('Error:', error);
                alert('Error executing query: ' + error.message);
            }
        }

        function displayResults(data) {
            if (!data.success) {
                alert('Error: ' + data.error.message);
                return;
            }

            const results = data.data;
            let html = '<h2>Search Results</h2>';

            if (results.rows.length === 0) {
                html += '<p>No results found</p>';
            } else {
                // Create table header
                html += '<table class="results-table">';
                html += '<thead><tr>';
                results.columns.forEach(column => {
                    html += `<th>${column}</th>`;
                });
                html += '</tr></thead>';

                // Create table body
                html += '<tbody>';
                results.rows.forEach(row => {
                    html += '<tr>';
                    row.forEach(cell => {
                        html += `<td>${cell ?? ''}</td>`;
                    });
                    html += '</tr>';
                });
                html += '</tbody></table>';

                // Add execution info
                html += `<div class="execution-info">
                    <p>Rows returned: ${results.rowCount}</p>
                    <p>Execution time: ${results.executionTime.toFixed(2)}s</p>
                </div>`;
            }

            searchResults.innerHTML = html;
            searchResults.style.display = 'block';
            searchResults.scrollIntoView({ behavior: 'smooth' });
        }
    }

    async function fetchAndDisplayTables(connectionId) {
        try {
            const response = await fetch(`/api/database/tables/${connectionId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch table names');
            }
            const data = await response.json();
            if (data.success && data.data.tables) {
                const tableNames = data.data.tables.join(', ');
                alert(`Tables in selected database: ${tableNames}`);
            }
        } catch (error) {
            console.error('Error fetching tables:', error);
            alert('Error fetching tables: ' + error.message);
        }
    }
}); 