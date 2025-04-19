using EchoSearch.Models;

namespace EchoSearch.Services;

public interface ISearchService
{
    IEnumerable<SearchResult> Search(string query);
}
