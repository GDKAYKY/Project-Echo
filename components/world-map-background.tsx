"use client"

import { useEffect, useRef } from "react"

export default function WorldMapBackground() {
  const canvasRef = useRef<HTMLCanvasElement>(null)

  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas) return

    const ctx = canvas.getContext("2d")
    if (!ctx) return

    // Handle resize
    const handleResize = () => {
      canvas.width = window.innerWidth
      canvas.height = window.innerHeight
      drawWorldMap(ctx, canvas.width, canvas.height)
    }

    window.addEventListener("resize", handleResize)
    handleResize() // Initial draw

    return () => {
      window.removeEventListener("resize", handleResize)
    }
  }, [])

  return <canvas ref={canvasRef} className="absolute inset-0 opacity-30" style={{ pointerEvents: "none" }} />
}

function drawWorldMap(ctx: CanvasRenderingContext2D, width: number, height: number) {
  ctx.clearRect(0, 0, width, height)

  // This is a simplified world map representation using dots
  // In a real implementation, you would use actual map coordinates

  // Define the map area (centered in the canvas)
  const mapWidth = width * 0.8
  const mapHeight = height * 0.7
  const mapX = (width - mapWidth) / 2
  const mapY = (height - mapHeight) / 2 + 50 // Offset a bit to match design

  // Draw dots
  ctx.fillStyle = "#444"

  // North America
  drawContinentDots(ctx, mapX + mapWidth * 0.2, mapY + mapHeight * 0.3, mapWidth * 0.2, mapHeight * 0.2, 0.5)

  // South America
  drawContinentDots(ctx, mapX + mapWidth * 0.25, mapY + mapHeight * 0.5, mapWidth * 0.15, mapHeight * 0.3, 0.5)

  // Europe
  drawContinentDots(ctx, mapX + mapWidth * 0.5, mapY + mapHeight * 0.25, mapWidth * 0.1, mapHeight * 0.15, 0.6)

  // Africa
  drawContinentDots(ctx, mapX + mapWidth * 0.5, mapY + mapHeight * 0.5, mapWidth * 0.15, mapHeight * 0.25, 0.5)

  // Asia
  drawContinentDots(ctx, mapX + mapWidth * 0.65, mapY + mapHeight * 0.3, mapWidth * 0.25, mapHeight * 0.25, 0.5)

  // Australia
  drawContinentDots(ctx, mapX + mapWidth * 0.8, mapY + mapHeight * 0.65, mapWidth * 0.1, mapHeight * 0.1, 0.5)
}

function drawContinentDots(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  width: number,
  height: number,
  density: number,
) {
  const dotCount = Math.floor(width * height * density * 0.05)

  for (let i = 0; i < dotCount; i++) {
    const dotX = x + Math.random() * width
    const dotY = y + Math.random() * height

    ctx.beginPath()
    ctx.arc(dotX, dotY, 1, 0, Math.PI * 2)
    ctx.fill()
  }
}
