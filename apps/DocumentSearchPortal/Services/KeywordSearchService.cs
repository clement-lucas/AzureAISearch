using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;


namespace DocumentSearchPortal.Services
{
    public class KeywordSearchService
    {
        private readonly SearchClient _searchClient;

        public KeywordSearchService(string serviceName, string indexName, string apiKey)
        {
            _searchClient = new SearchClient(new Uri($"https://{serviceName}.search.windows.net/"), indexName, new AzureKeyCredential(apiKey));
        }

        public async Task<SearchResults<SearchDocument>> KeywordSearchAsync(string searchText)
        {
            SearchOptions options = new SearchOptions
            {
                IncludeTotalCount = true,
                Filter = "",
                //OrderBy = { "metadata_storage_last_modified desc" }
            };

            options.HighlightFields.Add("content"); // Add fields to be highlighted  
            options.HighlightPreTag = "<b>";
            options.HighlightPostTag = "</b>";

            // Add fields to the select clause to limit fields in the results  
            options.Select.Add("Category");
            options.Select.Add("InformationId");
            options.Select.Add("ProtocolId");
            options.Select.Add("DisclosureScope");
            options.Select.Add("ModifiedBy");
            options.Select.Add("Description");

            // Default metadata
            options.Select.Add("metadata_storage_name");
            options.Select.Add("metadata_storage_path");
            options.Select.Add("metadata_storage_last_modified");
            options.Select.Add("metadata_storage_size");
            options.Select.Add("content");

            return await _searchClient.SearchAsync<SearchDocument>(searchText, options);
        }
    }
}
