document.addEventListener("DOMContentLoaded", () => {
  const canvas = document.getElementById("world-map-canvas")
  const ctx = canvas.getContext("2d")

  function resizeCanvas() {
    canvas.width = window.innerWidth - 128 // Adjust for sidebar
    canvas.height = window.innerHeight - 128 // Adjust for header
    drawWorldMap()
  }

  function drawWorldMap() {
    ctx.clearRect(0, 0, canvas.width, canvas.height)

    // Define the map area (centered in the canvas)
    const mapWidth = canvas.width * 0.8
    const mapHeight = canvas.height * 0.7
    const mapX = (canvas.width - mapWidth) / 2
    const mapY = (canvas.height - mapHeight) / 2 + 50 // Offset a bit to match design

    // Draw dots
    ctx.fillStyle = "#444"

    // North America
    drawContinentDots(mapX + mapWidth * 0.2, mapY + mapHeight * 0.3, mapWidth * 0.2, mapHeight * 0.2, 0.5)

    // South America
    drawContinentDots(mapX + mapWidth * 0.25, mapY + mapHeight * 0.5, mapWidth * 0.15, mapHeight * 0.3, 0.5)

    // Europe
    drawContinentDots(mapX + mapWidth * 0.5, mapY + mapHeight * 0.25, mapWidth * 0.1, mapHeight * 0.15, 0.6)

    // Africa
    drawContinentDots(mapX + mapWidth * 0.5, mapY + mapHeight * 0.5, mapWidth * 0.15, mapHeight * 0.25, 0.5)

    // Asia
    drawContinentDots(mapX + mapWidth * 0.65, mapY + mapHeight * 0.3, mapWidth * 0.25, mapHeight * 0.25, 0.5)

    // Australia
    drawContinentDots(mapX + mapWidth * 0.8, mapY + mapHeight * 0.65, mapWidth * 0.1, mapHeight * 0.1, 0.5)
  }

  function drawContinentDots(x, y, width, height, density) {
    const dotCount = Math.floor(width * height * density * 0.05)

    for (let i = 0; i < dotCount; i++) {
      const dotX = x + Math.random() * width
      const dotY = y + Math.random() * height

      ctx.beginPath()
      ctx.arc(dotX, dotY, 1, 0, Math.PI * 2)
      ctx.fill()
    }
  }

  // Initial setup
  window.addEventListener("resize", resizeCanvas)
  resizeCanvas()
})
