using Azure;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;


namespace DocumentSearchPortal.Services
{
    public class SearchService
    {
        private readonly SearchClient _keywordSearchClient;
        private readonly SearchClient _vectorSearchClient;
        private readonly SearchClient _hybridSearchClient;

        public SearchService(string serviceName, string keywordIndexName, string vectorIndexName, string hybridIndexName)
        {
            // Configure Managed Identity for the App service and use the App Service's Default Credentials (Managed Identity)
            _keywordSearchClient = new SearchClient(new Uri($"https://{serviceName}.search.windows.net/"), keywordIndexName, new DefaultAzureCredential());
            _vectorSearchClient = new SearchClient(new Uri($"https://{serviceName}.search.windows.net/"), vectorIndexName, new DefaultAzureCredential());
            _hybridSearchClient = new SearchClient(new Uri($"https://{serviceName}.search.windows.net/"), hybridIndexName, new DefaultAzureCredential());
        }

        private SearchOptions SetupCommonSearchOptions(SearchResultViewModel model)
        {
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
                    "metadata_storage_path",
                    "metadata_storage_last_modified",
                    "metadata_storage_size"
                },

            };

            if (!string.IsNullOrEmpty(model.FilterExpression))
            {
                options.Filter = model.FilterExpression;
            }

            if (!string.IsNullOrEmpty(model.OrderByExpression))
            {
                options.OrderBy.Add(model.OrderByExpression);
            }

            if (!string.IsNullOrEmpty(model.TopAISearch))
            {
                options.Size = Convert.ToInt32(model.TopAISearch);
            }

            if (model.SearchMode?.ToLower() == "all")
            {
                options.SearchMode = SearchMode.All;
            }

            return options;
        }

        public async Task<SearchResults<SearchDocument>> KeywordSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.HighlightFields.Add("content");

            return await _keywordSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> VectorSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("chunk");
            options.Select.Add("title");

            options.VectorSearch = new()
            {
                Queries = {
                        new VectorizableTextQuery(text: model.SearchQuery)
                        {

                            KNearestNeighborsCount = int.TryParse(model?.TopAISearch, out int parsedValue) ? parsedValue : 3,
                            Fields = { "vector" },
                            Exhaustive = true
                        }
                    },
            };

            return await _vectorSearchClient.SearchAsync<SearchDocument>(options);
        }

        public async Task<SearchResults<SearchDocument>> HybridSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("chunk");
            options.Select.Add("title");
            options.HighlightFields.Add("chunk");

            options.QueryType = SearchQueryType.Semantic;
            options.QueryLanguage = QueryLanguage.JaJp;
            options.SemanticSearch = new()
            {
                SemanticConfigurationName = "vectorsemantic-fju-nonprod-jpeast-01-semantic-configuration",
                QueryCaption = new(QueryCaptionType.Extractive),
                QueryAnswer = new(QueryAnswerType.Extractive),
                //SemanticQuery = model.SearchQuery,
            };

            options.VectorSearch = new()
            {
                Queries = {
                        new VectorizableTextQuery(text: model.SearchQuery)
                        {
                            KNearestNeighborsCount = int.TryParse(model?.TopAISearch, out int parsedValue) ? parsedValue : 3,
                            Fields = { "vector" },
                            Exhaustive = true
                        }
                    },
            };

            return await _hybridSearchClient.SearchAsync<SearchDocument>(model?.SearchQuery, options);
        }
    }
}
