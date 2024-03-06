namespace DocumentSearchPortal.Models
{
    public class SearchServiceConfig
    {
        public string ServiceName { get; set; } = string.Empty;
        public string KeywordIndexName { get; set; } = string.Empty;
        public string VectorIndexName { get; set; } = string.Empty;
        
        // Do not use ApiKey. Instead configure Managed Identity for the App service.
        //public string? ApiKey { get; set; } = string.Empty;
    }
}
