document.addEventListener('DOMContentLoaded', function() {
    console.log('Base64.js loaded successfully');
    
    // Elements
    const encodeUploadArea = document.getElementById('encode-upload-area');
    const encodeFileInput = document.getElementById('encode-file-input');
    const encodePreview = document.getElementById('encode-preview');
    const encodeCopyBtn = document.getElementById('encode-copy');
    const encodeBase64Text = document.getElementById('encode-base64-text');
    const encodeFileInfo = document.getElementById('encode-file-info');
    
    const decodeTextarea = document.getElementById('decode-base64-text');
    const decodePreview = document.getElementById('decode-preview');
    const decodeCopyBtn = document.getElementById('decode-copy');
    const decodeFileInfo = document.getElementById('decode-file-info');

    // Debug: Check if elements are found
    console.log('encodeUploadArea:', encodeUploadArea);
    console.log('encodeFileInput:', encodeFileInput);
    console.log('encodePreview:', encodePreview);
    console.log('encodeCopyBtn:', encodeCopyBtn);
    console.log('encodeBase64Text:', encodeBase64Text);
    console.log('encodeFileInfo:', encodeFileInfo);

    // Encode functionality
    if (encodeUploadArea && encodeFileInput) {
        console.log('Setting up encode event listeners');
        
        encodeUploadArea.addEventListener('click', () => {
            console.log('Upload area clicked');
            encodeFileInput.click();
        });
        
        encodeUploadArea.addEventListener('dragover', (e) => {
            console.log('Drag over detected');
            e.preventDefault();
            encodeUploadArea.classList.add('dragover');
        });

        encodeUploadArea.addEventListener('dragleave', () => {
            console.log('Drag leave detected');
            encodeUploadArea.classList.remove('dragover');
        });

        encodeUploadArea.addEventListener('drop', (e) => {
            console.log('File dropped');
            e.preventDefault();
            encodeUploadArea.classList.remove('dragover');
            const file = e.dataTransfer.files[0];
            if (file) {
                console.log('File dropped:', file.name);
                handleFileUpload(file);
            }
        });

        encodeFileInput.addEventListener('change', (e) => {
            console.log('File input changed');
            const file = e.target.files[0];
            if (file) {
                console.log('File selected:', file.name);
                handleFileUpload(file);
            }
        });
    } else {
        console.error('Required encode elements not found');
    }

    async function handleFileUpload(file) {
        console.log('handleFileUpload called with file:', file);
        
        if (!file.type.startsWith('image/')) {
            console.error('Invalid file type:', file.type);
            showError(encodeFileInfo, 'Please upload an image file');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        try {
            showLoading(encodeFileInfo, 'Processing...');
            console.log('Sending request to /api/base64/encode');
            
            const response = await fetch('/api/base64/encode', {
                method: 'POST',
                body: formData
            });

            console.log('Response status:', response.status);
            console.log('Response ok:', response.ok);

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Response error:', errorText);
                let errorData;
                try {
                    errorData = JSON.parse(errorText);
                } catch (e) {
                    errorData = { error: errorText };
                }
                throw new Error(errorData.error || 'Failed to encode image');
            }

            const result = await response.json();
            console.log('Success result:', result);
            
            encodeBase64Text.value = result.dataUrl;
            encodePreview.innerHTML = `<img src="${result.dataUrl}" class="preview-image" alt="Preview">`;
            showFileInfo(encodeFileInfo, file, result.sizeFormatted);
            
        } catch (error) {
            console.error('Error in handleFileUpload:', error);
            showError(encodeFileInfo, error.message);
        }
    }

    encodeCopyBtn.addEventListener('click', () => {
        copyToClipboard(encodeBase64Text.value, encodeCopyBtn);
    });

    // Decode functionality
    decodeTextarea.addEventListener('input', debounce(async () => {
        const base64 = decodeTextarea.value.trim();
        if (!base64) {
            decodePreview.innerHTML = '';
            decodeFileInfo.innerHTML = '';
            return;
        }

        try {
            showLoading(decodeFileInfo, 'Validating...');
            
            const response = await fetch('/api/base64/validate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ base64Data: base64 })
            });

            const result = await response.json();
            
            if (result.isValid) {
                decodePreview.innerHTML = `<img src="${base64}" class="preview-image" alt="Preview">`;
                showSuccess(decodeFileInfo, `Valid Base64 image (${result.sizeFormatted})`);
            } else {
                showError(decodeFileInfo, result.error || 'Invalid Base64 string');
                decodePreview.innerHTML = '';
            }
        } catch (error) {
            showError(decodeFileInfo, 'Failed to validate Base64');
            decodePreview.innerHTML = '';
        }
    }, 500));

    decodeCopyBtn.addEventListener('click', () => {
        copyToClipboard(decodeTextarea.value, decodeCopyBtn);
    });

    // Utility functions
    function showFileInfo(container, file, sizeFormatted) {
        container.innerHTML = `
            <p><strong>Name:</strong> ${file.name}</p>
            <p><strong>Type:</strong> ${file.type}</p>
            <p><strong>Size:</strong> ${sizeFormatted}</p>
        `;
    }

    function showLoading(container, message) {
        container.innerHTML = `<div class="loading">${message}</div>`;
    }

    function showError(container, message) {
        container.innerHTML = `<div class="error">${message}</div>`;
    }

    function showSuccess(container, message) {
        container.innerHTML = `<div class="success">${message}</div>`;
    }

    function copyToClipboard(text, button) {
        if (!text) return;

        const originalText = button.textContent;
        button.disabled = true;
        button.innerHTML = '<span class="loading"></span>';

        navigator.clipboard.writeText(text).then(() => {
            button.textContent = 'Copied!';
            setTimeout(() => {
                button.textContent = originalText;
                button.disabled = false;
            }, 2000);
        }).catch(() => {
            button.textContent = 'Failed to copy';
            setTimeout(() => {
                button.textContent = originalText;
                button.disabled = false;
            }, 2000);
        });
    }

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
}); 