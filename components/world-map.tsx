"use client"

import { useEffect, useRef } from "react"

export function WorldMapCanvas() {
  const canvasRef = useRef<HTMLCanvasElement>(null)

  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas) return

    const ctx = canvas.getContext("2d")
    if (!ctx) return

    // Set canvas dimensions
    canvas.width = 1000
    canvas.height = 500

    // Draw dot pattern for world map
    // This is a simplified example - in a real implementation,
    // you would use actual map coordinates to place dots

    ctx.fillStyle = "#444"

    // Generate random dots for demonstration
    // In a real implementation, you would use actual map coordinates
    for (let i = 0; i < 2000; i++) {
      const x = Math.random() * canvas.width
      const y = Math.random() * canvas.height

      // Skip drawing in ocean areas (simplified)
      if (Math.random() > 0.5) continue

      ctx.beginPath()
      ctx.arc(x, y, 1, 0, Math.PI * 2)
      ctx.fill()
    }
  }, [])

  return <canvas ref={canvasRef} className="w-full h-full" />
}
