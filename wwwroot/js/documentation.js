document.addEventListener('DOMContentLoaded', function() {
    try {
        // Configure marked with security options
        marked.setOptions({
            headerIds: true,
            mangle: false,
            breaks: true,
            gfm: true, // GitHub Flavored Markdown
            headerPrefix: 'doc-',
            langPrefix: 'language-',
            xhtml: false
        });

        // Get the markdown content from the model and parse it
        const markdownContent = JSON.parse(document.getElementById('markdown-content').getAttribute('data-content'));
        
        // Render the markdown content
        document.getElementById('markdown-content').innerHTML = marked.parse(markdownContent);
        
        // Generate table of contents
        generateTableOfContents();
        
        // Add copy button to code blocks
        addCopyButtonsToCodeBlocks();

        // Add syntax highlighting
        highlightCodeBlocks();
    } catch (error) {
        console.error('Error rendering markdown:', error);
        document.getElementById('markdown-content').innerHTML = 
            '<div class="error-message">Error loading documentation. Please try refreshing the page.</div>';
    }
});

function generateTableOfContents() {
    const content = document.getElementById('markdown-content');
    const headings = content.querySelectorAll('h2, h3');
    const toc = document.getElementById('table-of-contents');
    
    if (headings.length === 0) {
        toc.style.display = 'none';
        return;
    }

    const tocList = document.createElement('ul');
    tocList.className = 'toc-list';

    headings.forEach(heading => {
        const li = document.createElement('li');
        const a = document.createElement('a');
        a.href = '#' + heading.id;
        a.textContent = heading.textContent;
        a.className = heading.tagName.toLowerCase() === 'h3' ? 'toc-item toc-item-h3' : 'toc-item toc-item-h2';
        li.appendChild(a);
        tocList.appendChild(li);
    });

    toc.appendChild(tocList);
}

function addCopyButtonsToCodeBlocks() {
    const codeBlocks = document.querySelectorAll('pre code');
    codeBlocks.forEach(block => {
        const button = document.createElement('button');
        button.className = 'copy-button';
        button.textContent = 'Copy';
        button.addEventListener('click', () => {
            navigator.clipboard.writeText(block.textContent)
                .then(() => {
                    button.textContent = 'Copied!';
                    setTimeout(() => {
                        button.textContent = 'Copy';
                    }, 2000);
                })
                .catch(err => {
                    console.error('Failed to copy text: ', err);
                    button.textContent = 'Failed to copy';
                    setTimeout(() => {
                        button.textContent = 'Copy';
                    }, 2000);
                });
        });
        block.parentNode.style.position = 'relative';
        block.parentNode.appendChild(button);
    });
}

function highlightCodeBlocks() {
    const codeBlocks = document.querySelectorAll('pre code');
    codeBlocks.forEach(block => {
        // Add language class if not present
        if (!block.className) {
            block.className = 'language-plaintext';
        }
        
        // Add line numbers
        const lines = block.innerHTML.split('\n');
        const numberedLines = lines.map((line, index) => 
            `<span class="line-number">${index + 1}</span>${line}`
        ).join('\n');
        block.innerHTML = numberedLines;
    });
} 