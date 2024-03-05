using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;


namespace DocumentSearchPortal.Services
{
    public class KeywordSearchService
    {
        private readonly SearchClient _searchClient;

        public KeywordSearchService(string serviceName, string indexName, string apiKey)
        {
            _searchClient = new SearchClient(new Uri($"https://{serviceName}.search.windows.net/"), indexName, new AzureKeyCredential(apiKey));
        }

        public async Task<SearchResults<SearchDocument>> KeywordSearchAsync(SearchResultViewModel model)
        {
            //string filterExpression = "Category eq 'Category1'";//and metadata_storage_last_modified ge 2024-02-28T06:00:00Z

            SearchOptions options = new SearchOptions
            {                
                IncludeTotalCount = true,
                
                // Add fields to the select clause to limit fields in the results  
                Select = { 
                    "Category", 
                    "InformationId", 
                    "ProtocolId", 
                    "DisclosureScope", 
                    "ModifiedBy", 
                    "Description", 
                    "metadata_storage_name", 
                    "metadata_storage_path", 
                    "metadata_storage_last_modified",
                    "metadata_storage_size",
                    "content"
                },
                
                HighlightFields = {"content"}, // Add fields to be highlighted  
                //HighlightPreTag = "<b>",
                //HighlightPostTag = "</b>"                
            };

            if (!string.IsNullOrEmpty(model.FilterExpression))
            {
                options.Filter = model.FilterExpression; // Add filter expression.
            }

            if (!string.IsNullOrEmpty(model.OrderByExpression))
            {
                options.OrderBy.Add(model.OrderByExpression); // Add OrderBy expression.
            }

            if (!string.IsNullOrEmpty(model.Top))
            {
                options.Size = Convert.ToInt32(model.Top); // Add Top Count.
            }

            if (model.SearchMode?.ToLower() == "all")
            {
                options.SearchMode = SearchMode.All; // Use All to match all the keywords. By default, the search mode is Any.
            }

            return await _searchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }
    }
}
