document.addEventListener("DOMContentLoaded", () => {
  const searchInput = document.getElementById("search-input")
  const searchResults = document.getElementById("search-results")
  let searchTimeout

  // Handle cursor blinking
  const titleCursor = document.querySelector(".cursor")
  setInterval(() => {
    titleCursor.style.opacity = titleCursor.style.opacity === "0" ? "1" : "0"
  }, 530)

  // Handle sidebar item clicks
  const sidebarItems = document.querySelectorAll(".sidebar-item")
  sidebarItems.forEach((item) => {
    item.addEventListener("click", () => {
      // Remove active class from all items
      sidebarItems.forEach((i) => i.classList.remove("active"))
      // Add active class to clicked item
      item.classList.add("active")
    })
  })

  // Handle search input
  searchInput.addEventListener("input", function () {
    const query = this.value.trim()

    // Clear previous timeout
    if (searchTimeout) {
      clearTimeout(searchTimeout)
    }

    // Debounce search requests
    searchTimeout = setTimeout(() => {
      if (query.length === 0) {
        searchResults.innerHTML = ""
        return
      }

      // Show loading indicator
      searchResults.innerHTML = '<div class="result-item">Searching...</div>'

      // Fetch search results
      fetch(`/Home/Search?query=${encodeURIComponent(query)}`)
        .then((response) => response.json())
        .then((data) => {
          if (data.results.length === 0) {
            searchResults.innerHTML = '<div class="result-item">No results found</div>'
            return
          }

          // Display results
          let resultsHtml = ""
          data.results.forEach((result) => {
            resultsHtml += `
              <div class="result-item">
                <div class="result-name">${result.name}</div>
                <div class="result-location">${result.location}</div>
                <div class="result-details">${result.details}</div>
              </div>
            `
          })
          searchResults.innerHTML = resultsHtml
        })
        .catch((error) => {
          console.error("Error fetching search results:", error)
          searchResults.innerHTML = '<div class="result-item">Error fetching results</div>'
        })
    }, 300)
  })

  // Update placeholder with blinking cursor
  function updatePlaceholder() {
    const currentPlaceholder = searchInput.placeholder
    if (currentPlaceholder === "Search_") {
      searchInput.placeholder = "Search "
    } else {
      searchInput.placeholder = "Search_"
    }
  }

  setInterval(updatePlaceholder, 530)
})
