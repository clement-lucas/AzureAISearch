using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using System.Reflection;


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
        public async Task<SearchResults<SearchDocument>> VectorSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = new SearchOptions
            {
                IncludeTotalCount = true,            
                
                // Add fields to the select clause to limit fields in the results  
                Select = {
                    "title",
                    "chunk",
                    "Category",
                    "InformationId",
                    "ProtocolId",
                    "DisclosureScope",
                    "ModifiedBy",
                    "Description",
                    "metadata_storage_path",
                    "metadata_storage_last_modified",
                    "metadata_storage_size"
                },
                
                //HighlightFields = { "chunk" }, // Add fields to be highlighted  
                //HighlightPreTag = "<b>",
                //HighlightPostTag = "</b>"
                
                VectorSearch = new()
                {
                    Queries = {
                        new VectorizableTextQuery(text: model.SearchQuery)
                        {
                            //KNearestNeighborsCount = 5,
                            Fields = { "vector" },
                            Exhaustive = false
                        }
                    },
                }
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
                options.VectorSearch.Queries.First().KNearestNeighborsCount = Convert.ToInt32(model.Top); // Add Top Count.
            }

            if (model.SearchMode?.ToLower() == "all")
            {
                options.SearchMode = SearchMode.All; // Use All to match all the keywords. By default, the search mode is Any.
            }

            return await _searchClient.SearchAsync<SearchDocument>(options);
        }
    }
}
