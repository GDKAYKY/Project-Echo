document.addEventListener('DOMContentLoaded', function() {
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

    // Encode functionality
    encodeUploadArea.addEventListener('click', () => encodeFileInput.click());
    
    encodeUploadArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        encodeUploadArea.classList.add('dragover');
    });

    encodeUploadArea.addEventListener('dragleave', () => {
        encodeUploadArea.classList.remove('dragover');
    });

    encodeUploadArea.addEventListener('drop', (e) => {
        e.preventDefault();
        encodeUploadArea.classList.remove('dragover');
        const file = e.dataTransfer.files[0];
        if (file) handleFileUpload(file);
    });

    encodeFileInput.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (file) handleFileUpload(file);
    });

    function handleFileUpload(file) {
        if (!file.type.startsWith('image/')) {
            showError(encodeFileInfo, 'Please upload an image file');
            return;
        }

        const reader = new FileReader();
        reader.onload = function(e) {
            const base64 = String(e.target.result);
            encodeBase64Text.value = base64;
            encodePreview.innerHTML = `<img src="${base64}" class="preview-image" alt="Preview">`;
            showFileInfo(encodeFileInfo, file);
        };
        reader.readAsDataURL(file);
    }

    encodeCopyBtn.addEventListener('click', () => {
        copyToClipboard(encodeBase64Text.value, encodeCopyBtn);
    });

    // Decode functionality
    decodeTextarea.addEventListener('input', () => {
        const base64 = String(decodeTextarea.value.trim());
        if (!base64) {
            decodePreview.innerHTML = '';
            decodeFileInfo.innerHTML = '';
            return;
        }

        try {
            if (!isValidBase64(base64)) {
                throw new Error('Invalid Base64 string');
            }

            decodePreview.innerHTML = `<img src="${base64}" class="preview-image" alt="Preview">`;
            showSuccess(decodeFileInfo, 'Valid Base64 image');
        } catch (error) {
            showError(decodeFileInfo, error.message);
            decodePreview.innerHTML = '';
        }
    });

    decodeCopyBtn.addEventListener('click', () => {
        copyToClipboard(decodeTextarea.value, decodeCopyBtn);
    });

    // Utility functions
    function showFileInfo(container, file) {
        const size = formatFileSize(file.size);
        container.innerHTML = `
            <p><strong>Name:</strong> ${file.name}</p>
            <p><strong>Type:</strong> ${file.type}</p>
            <p><strong>Size:</strong> ${size}</p>
        `;
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

    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    function isValidBase64(str) {
        try {
            return btoa(atob(str.split(',')[1] || str)) === (str.split(',')[1] || str);
        } catch (err) {
            console.error("Base64 validation error:", err);
            return false;
        }
    }
}); 