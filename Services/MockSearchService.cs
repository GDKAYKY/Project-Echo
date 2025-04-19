using EchoSearch.Models;

namespace EchoSearch.Services;

public class MockSearchService : ISearchService
{
    private readonly List<SearchResult> _database = new()
    {
        new SearchResult { Id = 1, Name = "John Doe", Location = "New York, USA", Details = "Software Engineer" },
        new SearchResult { Id = 2, Name = "Jane Smith", Location = "London, UK", Details = "Data Scientist" },
        new SearchResult { Id = 3, Name = "Ahmed Khan", Location = "Dubai, UAE", Details = "Business Analyst" },
        new SearchResult { Id = 4, Name = "Maria Garcia", Location = "Madrid, Spain", Details = "UX Designer" },
        new SearchResult { Id = 5, Name = "Liu Wei", Location = "Beijing, China", Details = "Product Manager" }
    };

    public IEnumerable<SearchResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<SearchResult>();
        }

        query = query.ToLowerInvariant();

        return _database.Where(item =>
            item.Name.ToLowerInvariant().Contains(query) ||
            item.Location.ToLowerInvariant().Contains(query) ||
            item.Details.ToLowerInvariant().Contains(query)
        );
    }
}
