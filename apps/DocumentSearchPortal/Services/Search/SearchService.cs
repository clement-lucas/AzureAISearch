using Azure;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using Microsoft.Extensions.Options;


namespace DocumentSearchPortal.Services.Search
{
    /// <summary>
    /// SearchService
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly SearchClient _keywordSearchClient;
        private readonly SearchClient _vectorSearchClient;
        private readonly SearchClient _hybridSearchClient;
        private readonly SearchClient _combinedSearchClient;
        private readonly SearchServiceConfig _config;

        /// <summary>
        /// SearchService
        /// </summary>
        /// <param name="options"></param>
        public SearchService(IOptions<SearchServiceConfig> options)
        {
            _config = options.Value;

            // Assuming Managed Identity is configured correctly in the Azure service.
            _keywordSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"), _config.KeywordIndexName, new DefaultAzureCredential());
            _vectorSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"), _config.VectorIndexName, new DefaultAzureCredential());
            _hybridSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"), _config.HybridIndexName, new DefaultAzureCredential());
            _combinedSearchClient = new SearchClient(new Uri($"https://{options.Value.ServiceName}.search.windows.net/"), _config.CombinedIndexName, new DefaultAzureCredential());
        }

        /// <summary>
        /// PerformSearch
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResultViewModel</returns>
        public async Task<SearchResultViewModel> PerformSearch(SearchResultViewModel model)
        {
            if (model.SelectedIndexes.Count == 0)
            {
                return model;
            }

            if (model.SelectedIndexes.Contains("Normal"))
            {
                model.NormalSearchResults = await KeywordSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Vector"))
            {
                model.VectorSearchResults = await VectorSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Hybrid"))
            {
                model.HybridSearchResults = await HybridSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("SQL+Normal"))
            {
                model.CombinedSearchResults = await CombinedSearchAsync(model);
            } 

            return model;
        }

        /// <summary>
        /// SetupCommonSearchOptions
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchOptions</returns>
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

            if (model.CountSearchResult.HasValue && model.CountSearchResult > 0)
            {
                options.Size = Convert.ToInt32(model.CountSearchResult);
            }

            if (model.SearchMode?.ToLower() == "all")
            {
                options.SearchMode = SearchMode.All;
            }

            return options;
        }

        /// <summary>
        /// KeywordSearchAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResults<SearchDocument></returns>
        public async Task<SearchResults<SearchDocument>> KeywordSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _keywordSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        /// <summary>
        /// VectorSearchAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResults<SearchDocument></returns>
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
                            KNearestNeighborsCount = (model?.CountVectorResult.HasValue ?? false) && model.CountVectorResult > 0 ? model.CountVectorResult : null,
                            Fields = { "vector" },
                            Exhaustive = true
                        }
                    },
            };

            return await _vectorSearchClient.SearchAsync<SearchDocument>(options);
        }

        /// <summary>
        /// HybridSearchAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResults<SearchDocument></returns>
        public async Task<SearchResults<SearchDocument>> HybridSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("chunk");
            options.Select.Add("title");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"chunk-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("chunk");
            }

            options.QueryType = SearchQueryType.Semantic;
            options.QueryLanguage = QueryLanguage.JaJp;
            options.SemanticSearch = new()
            {
                SemanticConfigurationName = _config.SemanticConfigurationName,
                QueryCaption = new(QueryCaptionType.Extractive),
                QueryAnswer = new(QueryAnswerType.Extractive),
            };

            options.VectorSearch = new()
            {
                Queries = {
                        new VectorizableTextQuery(text: model.SearchQuery)
                        {
                            KNearestNeighborsCount = (model?.CountVectorResult.HasValue ?? false) && model.CountVectorResult > 0 ? model.CountVectorResult : null,
                            Fields = { "vector" },
                            Exhaustive = true
                        }
                    },
            };

            return await _hybridSearchClient.SearchAsync<SearchDocument>(model?.SearchQuery, options);
        }

        /// <summary>
        /// CombinedSearchAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResults<SearchDocument></returns>
        public async Task<SearchResults<SearchDocument>> CombinedSearchAsync(SearchResultViewModel model)
        {
            // Set up common search options  
            SearchOptions options = SetupCommonSearchOptions(model);
            // Customize the options.Select for the fields specific to the combined index  
            options.Select.Clear();
            options.Select.Add("DocumentId");
            options.Select.Add("FileName");
            options.Select.Add("Description");
            options.Select.Add("Version");
            //.Select.Add("BlobStorageUri");
            options.Select.Add("CategoryName");
            options.Select.Add("ProtocolName");
            options.Select.Add("CreatedByName");
            options.Select.Add("ModifiedByName");
            options.Select.Add("DisclosureScopeName");
            options.Select.Add("InformationName");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("metadata_storage_path");
            options.Select.Add("metadata_storage_last_modified");
            options.Select.Add("metadata_storage_size");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            // Perform the search  
            var searchResults = await _combinedSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);

            return searchResults;
        }
    }
}
