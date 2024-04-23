namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// SearchServiceConfig
    /// </summary>
    public class SearchServiceConfig
    {
        public string ServiceName { get; set; } = string.Empty;
        public string IndexNameKeyword { get; set; } = string.Empty;
        public string IndexNameVector { get; set; } = string.Empty;
        public string IndexNameVectorSemantic { get; set; } = string.Empty;
        public string IndexNameHybridVectorAzFunc { get; set; } = string.Empty;
        public string IndexNameCombined { get; set; } = string.Empty;        
        public string IndexNameSecurity { get; set; } = string.Empty;
        public string IndexNameAIEnrichImage { get; set; } = string.Empty;
        public string IndexNameAIEnrichCustomEntityLookup { get; set; } = string.Empty;
        public string IndexNameAIEnrichEntityLinking { get; set; } = string.Empty;
        public string IndexNameAIEnrichEntityRecognition { get; set; } = string.Empty;
        public string IndexNameAIEnrichKeyPhraseExtraction { get; set; } = string.Empty;
        public string IndexNameAIEnrichLanguageDetection { get; set; } = string.Empty;
        public string IndexNameAIEnrichPIIDetection { get; set; } = string.Empty;
        public string CombinedIndexer1Name { get; set; } = string.Empty;
        public string CombinedIndexer2Name { get; set; } = string.Empty;
        public string AzureBlobStorageConnectionString { get; set; } = string.Empty;
        public string NormalContainerName { get; set; } = string.Empty;
        public string SQLContainerName { get; set; } = string.Empty;
        public string SQLDbConnectionString { get; set; } = string.Empty;
        public string SemanticConfigurationName { get; set; } = string.Empty;
        public string SemanticConfigurationVectorAzFuncName { get; set; } = string.Empty;
    }
}
