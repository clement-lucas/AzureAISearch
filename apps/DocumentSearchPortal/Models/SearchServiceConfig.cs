namespace DocumentSearchPortal.Models
{
    /// <summary>
    /// SearchServiceConfig
    /// </summary>
    public class SearchServiceConfig
    {
        public string ServiceName { get; set; } = string.Empty;
        public string KeywordIndexName { get; set; } = string.Empty;
        public string VectorIndexName { get; set; } = string.Empty;
        public string HybridIndexName { get; set; } = string.Empty;
        public string HybridVectorAzFuncIndexName { get; set; } = string.Empty;
        public string CombinedIndexName { get; set; } = string.Empty;
        public string CombinedIndexer1Name { get; set; } = string.Empty;
        public string CombinedIndexer2Name { get; set; } = string.Empty;
        public string SecurityIndexName { get; set; } = string.Empty;
        public string AzureBlobStorageConnectionString { get; set; } = string.Empty;
        public string NormalContainerName { get; set; } = string.Empty;
        public string SQLContainerName { get; set; } = string.Empty;
        public string SQLDbConnectionString { get; set; } = string.Empty;
        public string SemanticConfigurationName { get; set; } = string.Empty;
        public string SemanticConfigurationVectorAzFuncName { get; set; } = string.Empty;
    }
}
