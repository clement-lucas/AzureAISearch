using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using System.Threading.Tasks;

namespace DocumentSearchPortal.Services.Search
{
    public interface ISearchClientWrapper
    {
        Task<SearchResults<SearchDocument>> SearchAsync(string searchQuery, SearchOptions options, string indexName);
    }
}
