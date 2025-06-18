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
                uploadPopup.style.display = 'flex';
                openUploadPopupBtn.classList.add('active');
            });

            closePopupBtn.addEventListener('click', function() {
                uploadPopup.style.display = 'none';
                openUploadPopupBtn.classList.remove('active');
            });

            window.addEventListener('click', function(event) {
                if (event.target === uploadPopup) {
                    uploadPopup.style.display = 'none';
                    openUploadPopupBtn.classList.remove('active');
                }
            });
        }

        // Drag and Drop functionality
        const dragDropArea = document.getElementById('drag-drop-area');
        const dbFile = document.getElementById('dbFile');
        const fileNameSpan = document.getElementById('file-name');

        if (dragDropArea && dbFile && fileNameSpan) {
            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                dragDropArea.addEventListener(eventName, preventDefaults, false);
            });

            ['dragenter', 'dragover'].forEach(eventName => {
                dragDropArea.addEventListener(eventName, highlight, false);
            });

            ['dragleave', 'drop'].forEach(eventName => {
                dragDropArea.addEventListener(eventName, unhighlight, false);
            });

            dragDropArea.addEventListener('drop', handleDrop, false);

            dbFile.addEventListener('change', function() {
                if (dbFile.files.length > 0) {
                    fileNameSpan.textContent = dbFile.files[0].name;
                } else {
                    fileNameSpan.textContent = 'Escolher arquivo';
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

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        function highlight() {
            dragDropArea.classList.add('highlight');
        }

        function unhighlight() {
            dragDropArea.classList.remove('highlight');
        }

        function handleDrop(e) {
            const dt = e.dataTransfer;
            const files = dt.files;
            if (files.length > 0) {
                dbFile.files = files;
                fileNameSpan.textContent = files[0].name;
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
                // Check if there's a BASE64 column
                const base64ColumnIndex = results.columns.findIndex(col => col.toUpperCase() === 'BASE64');
                const hasBase64Column = base64ColumnIndex !== -1;

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
                    row.forEach((cell, index) => {
                        if (hasBase64Column && index === base64ColumnIndex && cell) {
                            // Display image for BASE64 column
                            html += `<td class="base64-cell">
                                <div class="base64-preview">
                                    <img src="data:image/png;base64,${cell}" alt="Base64 Image" 
                                         onerror="this.style.display='none'" 
                                         onclick="showFullImage(this.src)" />
                                </div>
                            </td>`;
                        } else {
                            html += `<td>${cell ?? ''}</td>`;
                        }
                    });
                    html += '</tr>';
                });
                html += '</tbody></table>';

                // Add execution info
                html += `<div class="execution-info">
                    <p>Rows returned: ${results.rowCount}</p>
                    <p>Execution time: ${results.executionTime.toFixed(2)}s</p>
                </div>`;

                // Add image preview modal if we have BASE64 data
                if (hasBase64Column) {
                    html += `
                    <div id="imagePreviewModal" class="modal">
                        <span class="close-modal">&times;</span>
                        <img class="modal-content" id="previewImage">
                    </div>`;
                }
            }

            searchResults.innerHTML = html;
            searchResults.style.display = 'block';
            searchResults.scrollIntoView({ behavior: 'smooth' });

            // Add modal event listeners if modal exists
            const modal = document.getElementById('imagePreviewModal');
            if (modal) {
                const closeBtn = modal.querySelector('.close-modal');
                closeBtn.onclick = function() {
                    modal.style.display = "none";
                }
                window.onclick = function(event) {
                    if (event.target == modal) {
                        modal.style.display = "none";
                    }
                }
            }
        }

        // Function to show full image in modal
        window.showFullImage = function(src) {
            const modal = document.getElementById('imagePreviewModal');
            const modalImg = document.getElementById('previewImage');
            modal.style.display = "block";
            modalImg.src = src;
        }
    }
}); 