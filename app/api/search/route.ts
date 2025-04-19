import { NextResponse } from "next/server"

// This would connect to your actual database in a real implementation
const mockDatabase = [
  { id: 1, name: "John Doe", location: "New York, USA", details: "Software Engineer" },
  { id: 2, name: "Jane Smith", location: "London, UK", details: "Data Scientist" },
  { id: 3, name: "Ahmed Khan", location: "Dubai, UAE", details: "Business Analyst" },
  { id: 4, name: "Maria Garcia", location: "Madrid, Spain", details: "UX Designer" },
  { id: 5, name: "Liu Wei", location: "Beijing, China", details: "Product Manager" },
]

export async function GET(request: Request) {
  const { searchParams } = new URL(request.url)
  const query = searchParams.get("q")?.toLowerCase() || ""

  // Simple search implementation
  const results = mockDatabase.filter(
    (item) =>
      item.name.toLowerCase().includes(query) ||
      item.location.toLowerCase().includes(query) ||
      item.details.toLowerCase().includes(query),
  )

  // Simulate network delay
  await new Promise((resolve) => setTimeout(resolve, 500))

  return NextResponse.json({ results })
}
