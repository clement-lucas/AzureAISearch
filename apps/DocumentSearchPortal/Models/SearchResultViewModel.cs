using Azure.Search.Documents.Models;

namespace DocumentSearchPortal.Models
{
    public class SearchResultViewModel
    {
        public string SearchQuery { get; set; } = "*";

        public string? FilterExpression { get; set; }

        public string? OrderByExpression { get; set; }

        public string? Top { get; set; }

        public string? SearchMode { get; set; }

        public SearchResults<SearchDocument>? NormalSearchResults { get; set; }

        public SearchResults<SearchDocument>? VectorSearchResults { get; set; }  
    }
}
