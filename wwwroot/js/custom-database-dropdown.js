document.addEventListener('DOMContentLoaded', function() {
    const dropdown = document.getElementById('custom-database-dropdown');
    if (!dropdown) return;
    const selected = dropdown.querySelector('.custom-dropdown-selected');
    const list = dropdown.querySelector('.custom-dropdown-list');
    const options = Array.from(list.querySelectorAll('li'));
    const hiddenInput = dropdown.querySelector('input[type="hidden"]');

    // Open/close dropdown
    function openDropdown() {
        list.style.display = 'block';
        dropdown.classList.add('open');
    }
    function closeDropdown() {
        list.style.display = 'none';
        dropdown.classList.remove('open');
    }
    selected.addEventListener('click', function(e) {
        e.stopPropagation();
        if (list.style.display === 'block') {
            closeDropdown();
        } else {
            openDropdown();
        }
    });
    // Select option
    options.forEach(option => {
        option.addEventListener('click', function(e) {
            e.stopPropagation();
            options.forEach(o => o.classList.remove('selected'));
            option.classList.add('selected');
            selected.textContent = option.textContent;
            hiddenInput.value = option.getAttribute('data-value');
            closeDropdown();
        });
    });
    // Close on click outside
    document.addEventListener('click', function(e) {
        if (!dropdown.contains(e.target)) {
            closeDropdown();
        }
    });
    // Keyboard navigation
    let focusIdx = -1;
    dropdown.addEventListener('keydown', function(e) {
        if (list.style.display !== 'block') {
            if (e.key === 'Enter' || e.key === ' ' || e.key === 'ArrowDown') {
                openDropdown();
                focusIdx = options.findIndex(o => o.classList.contains('selected'));
                if (focusIdx < 0) focusIdx = 0;
                options[focusIdx].focus();
                e.preventDefault();
            }
            return;
        }
        if (e.key === 'ArrowDown') {
            focusIdx = (focusIdx + 1) % options.length;
            options[focusIdx].focus();
            e.preventDefault();
        } else if (e.key === 'ArrowUp') {
            focusIdx = (focusIdx - 1 + options.length) % options.length;
            options[focusIdx].focus();
            e.preventDefault();
        } else if (e.key === 'Enter') {
            options[focusIdx].click();
            e.preventDefault();
        } else if (e.key === 'Escape') {
            closeDropdown();
            e.preventDefault();
        }
    });
    // Make options focusable
    options.forEach(o => o.tabIndex = 0);
}); 