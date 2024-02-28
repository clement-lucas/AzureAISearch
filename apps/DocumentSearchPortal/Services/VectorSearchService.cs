using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;


namespace DocumentSearchPortal.Services
{
    public class VectorSearchService
    {
        private readonly SearchClient _searchClient;

        public VectorSearchService(string serviceName, string indexName, string apiKey)
        {
            _searchClient = new SearchClient(new Uri($"https://{serviceName}.search.windows.net/"), indexName, new AzureKeyCredential(apiKey));
        }

        // Method for vector search  
        public async Task<SearchResults<SearchDocument>> VectorSearchAsync(string searchText)
        {
            SearchOptions options = new SearchOptions
            {
                IncludeTotalCount = true,
                Filter = "",
                // Add other search options as needed  
            };

            options.Select.Add("chunk");
            options.Select.Add("title");
            //options.Select.Add("metadata_storage_name");
            //options.Select.Add("metadata_storage_last_modified");
            //options.Select.Add("Description");
            //options.Select.Add("Category");
            //options.Select.Add("InformationId");
            //options.Select.Add("ProtocolId");
            //options.Select.Add("DisclosureScope");
            //options.Select.Add("ModifiedBy");

            return await _searchClient.SearchAsync<SearchDocument>(searchText, options);
        }
    }
}
