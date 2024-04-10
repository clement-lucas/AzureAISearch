using Azure.Search.Documents.Models;

namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// SearchResultViewModel
    /// </summary>
    public class SearchResultViewModel
    {
        public string SearchQuery { get; set; } = "*";

        public string? FilterExpression { get; set; }

        public string? OrderByExpression { get; set; }

        public int? CountSearchResult { get; set; }

        public int? CountHighlightResult { get; set; }

        public int? CountVectorResult { get; set; }

        public int? CountPrefixSuffix { get; set; }

        public string? SearchMode { get; set; } 

        public SearchResults<SearchDocument>? NormalSearchResults { get; set; }

        public SearchResults<SearchDocument>? VectorSearchResults { get; set; }

        public SearchResults<SearchDocument>? HybridAdaSearchResults { get; set; }

        public SearchResults<SearchDocument>? HybridCustomVectorSearchResults { get; set; }

        public SearchResults<SearchDocument>? CombinedSearchResults { get; set; }

        public SearchResults<SearchDocument>? SecuritySearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichmentSearchResults { get; set; }

        public List<string> AvailableIndexes { get; set; } = new List<string> { 
            "Normal", 
            "Vector", 
            "Hybrid", 
            "Hybrid-CustomVector", 
            "SQL+Normal", 
            "Document Security",
            "Normal + AI Enrichment (Image)"
        };

        public List<string> SelectedIndexes { get; set; } = new List<string> { "Normal" };


    }
}
