// Convert page interactions: robust validation, loading states, and UX helpers
document.addEventListener('DOMContentLoaded', function () {
    const alertBox = document.querySelector('.alert');

    function showError(message) {
        if (!alertBox) return;
        alertBox.textContent = message || 'An error occurred.';
        alertBox.style.display = 'block';
    }

    function clearError() {
        if (!alertBox) return;
        alertBox.textContent = '';
        alertBox.style.display = 'none';
    }

    function setLoading(button, isLoading) {
        if (!button) return;
        if (isLoading) {
            button.dataset.originalText = button.innerHTML;
            button.innerHTML = 'Processing…';
            button.disabled = true;
        } else {
            if (button.dataset.originalText) button.innerHTML = button.dataset.originalText;
            button.disabled = false;
        }
    }

    function isValidUrl(value) {
        try {
            const u = new URL(value);
            return !!u.protocol && !!u.host;
        } catch {
            return false;
        }
    }

    // Download by URL form
    const downloadForm = document.querySelector('form [name="handler"][value="Download"]')?.closest('form');
    if (downloadForm) {
        const urlInput = downloadForm.querySelector('#Url');
        const typeSelect = downloadForm.querySelector('#DownloadType');
        const formatSelect = downloadForm.querySelector('#Format');
        const submitBtn = downloadForm.querySelector('button[type="submit"]');

        function validateDownload() {
            clearError();
            if (!urlInput || !urlInput.value.trim()) {
                showError('URL is required.');
                urlInput?.focus();
                return false;
            }
            if (!isValidUrl(urlInput.value.trim())) {
                showError('Enter a valid URL (https://...)');
                urlInput?.focus();
                return false;
            }
            if (typeSelect && formatSelect) {
                const t = (typeSelect.value || '').toLowerCase();
                const f = (formatSelect.value || '').toLowerCase();
                const audioFormats = new Set(['mp3', 'opus']);
                const videoFormats = new Set(['mp4', 'webm']);
                if (t === 'audio' && !(audioFormats.has(f) || f === 'best')) {
                    showError('For audio downloads, choose mp3, opus, or best.');
                    formatSelect.focus();
                    return false;
                }
                if (t === 'video' && !(videoFormats.has(f) || f === 'best')) {
                    showError('For video downloads, choose mp4, webm, or best.');
                    formatSelect.focus();
                    return false;
                }
            }
            return true;
        }

        // Sync constraints when type changes
        typeSelect?.addEventListener('change', function () {
            if (!formatSelect) return;
            const t = (typeSelect.value || '').toLowerCase();
            const f = (formatSelect.value || '').toLowerCase();
            if (t === 'audio' && !['mp3', 'opus', 'best'].includes(f)) {
                formatSelect.value = 'mp3';
            }
            if (t === 'video' && !['mp4', 'webm', 'best'].includes(f)) {
                formatSelect.value = 'mp4';
            }
        });

        downloadForm.addEventListener('submit', function () {
            if (!validateDownload()) return false;
            setLoading(submitBtn, true);
            setTimeout(() => setLoading(submitBtn, false), 4000);
        });
    }

    // Convert local file form
    const convertForm = document.querySelector('form [name="handler"][value="Convert"]')?.closest('form');
    if (convertForm) {
        const fileInput = convertForm.querySelector('#Upload');
        const targetFormat = convertForm.querySelector('#TargetFormat');
        const submitBtn2 = convertForm.querySelector('button[type="submit"]');

        // Show selected file name
        fileInput?.addEventListener('change', function () {
            clearError();
        });

        function validateFile() {
            clearError();
            if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
                showError('Please choose a file to convert.');
                fileInput?.focus();
                return false;
            }
            const f = fileInput.files[0];
            // Server allows up to 300MB (see Program.cs upload limits)
            const max = 300 * 1024 * 1024;
            if (f.size > max) {
                showError('File too large. Max 300MB.');
                return false;
            }
            // Hint: if an audio-only target is chosen but source seems video, we still allow ffmpeg to handle it.
            if (!targetFormat || !targetFormat.value) {
                showError('Choose a target format.');
                targetFormat?.focus();
                return false;
            }
            return true;
        }

        convertForm.addEventListener('submit', function () {
            if (!validateFile()) return false;
            setLoading(submitBtn2, true);
            setTimeout(() => setLoading(submitBtn2, false), 4000);
        });
    }
});


