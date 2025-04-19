"use client"

import { useState, useEffect } from "react"

type SearchResult = {
  id: number
  name: string
  location: string
  details: string
}

export default function SearchResults({ query }: { query: string }) {
  const [results, setResults] = useState<SearchResult[]>([])
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (!query.trim()) {
      setResults([])
      return
    }

    const fetchResults = async () => {
      setLoading(true)
      try {
        const response = await fetch(`/api/search?q=${encodeURIComponent(query)}`)
        const data = await response.json()
        setResults(data.results)
      } catch (error) {
        console.error("Error fetching search results:", error)
      } finally {
        setLoading(false)
      }
    }

    // Debounce search requests
    const timer = setTimeout(() => {
      fetchResults()
    }, 300)

    return () => clearTimeout(timer)
  }, [query])

  if (loading) {
    return <div className="mt-4 text-gray-400">Searching...</div>
  }

  if (query.trim() && results.length === 0) {
    return <div className="mt-4 text-gray-400">No results found</div>
  }

  if (results.length === 0) {
    return null
  }

  return (
    <div className="mt-4 bg-[#1e1e1e] rounded-sm p-4 max-w-md">
      <h3 className="text-sm text-gray-400 mb-2">Results</h3>
      <ul className="space-y-3">
        {results.map((result) => (
          <li key={result.id} className="border-b border-gray-800 pb-2 last:border-0 last:pb-0">
            <div className="font-medium">{result.name}</div>
            <div className="text-sm text-gray-400">{result.location}</div>
            <div className="text-xs text-gray-500 mt-1">{result.details}</div>
          </li>
        ))}
      </ul>
    </div>
  )
}
