using Azure.Search.Documents.Models;
using System.Text.Json.Serialization;

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

        public SearchResults<SearchDocument>? NormalJapaneseSearchResults { get; set; }

        public SearchResults<SearchDocument>? NormalEnglishSearchResults { get; set; }

        public SearchResults<SearchDocument>? NormalStandardSearchResults { get; set; }

        public SearchResults<SearchDocument>? MultiLanguageSearchResults { get; set; }

        public SearchResults<SearchDocument>? VectorSearchResults { get; set; }

        public SearchResults<SearchDocument>? HybridAdaSearchResults { get; set; }

        public SearchResults<SearchDocument>? NewHybridSearchResults { get; set; }

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

        public SearchResults<SearchDocument>? AIEnrichTranslationSearchResults { get; set; }

        public SearchResults<SearchDocument>? AIEnrichSentimentSearchResults { get; set; }

        public List<string> AvailableIndexes { get; set; } = new List<string> {
            "Normal - Japanese Analyzer",
            "Normal - English Analyzer",
            "Normal - Standard Analyzer",
            "Multi-Language",
            "Vector",
            "Hybrid",
            "New Hybrid",
            "Hybrid-CustomVector",
            "SQL+Normal",
            "Document Security",
            "AI Enrichment - Image",
            "AI Enrichment - Custom Entity Lookup",
            "AI Enrichment - Entity Linking",
            "AI Enrichment - Entity Recognition",
            "AI Enrichment - Key Phrase Extraction",
            "AI Enrichment - Language Detection",
            "AI Enrichment - PII Detection",
            "AI Enrichment - Text Translation",
            "AI Enrichment - Sentiment"
        };

        public List<string> SelectedIndexes { get; set; } = new List<string> { "Normal" };

    }

    public class Sentence
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("sentiment")]
        public string Sentiment { get; set; }

        [JsonPropertyName("confidenceScores")]
        public ConfidenceScores ConfidenceScores { get; set; }
    }

    public class ConfidenceScores
    {
        [JsonPropertyName("positive")]
        public double Positive { get; set; }

        [JsonPropertyName("neutral")]
        public double Neutral { get; set; }

        [JsonPropertyName("negative")]
        public double Negative { get; set; }
    }
}
