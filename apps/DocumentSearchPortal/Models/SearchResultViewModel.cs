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

        public SearchResults<SearchDocument>? AIEnrichImageSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichCustomEntityLookupSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichEntityLinkingSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichEntityRecognitionSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichKeyPhraseExtractionSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichLanguageDetectionSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichPIIDetectionSearchResults { get; set; }

        public List<string> AvailableIndexes { get; set; } = new List<string> { 
            "Normal", 
            "Vector", 
            "Hybrid", 
            "Hybrid-CustomVector", 
            "SQL+Normal", 
            "Document Security",
            "AI Enrichment - Image",
            "AI Enrichment - Custom Entity Lookup",
            "AI Enrichment - Entity Linking",
            "AI Enrichment - Entity Recognition",
            "AI Enrichment - Key Phrase Extraction",
            "AI Enrichment - Language Detection",
            "AI Enrichment - PII Detection"

        };

        public List<string> SelectedIndexes { get; set; } = new List<string> { "Normal" };


    }
}
