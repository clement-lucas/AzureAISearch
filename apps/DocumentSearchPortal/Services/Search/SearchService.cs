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
        private readonly SearchClient _hybridCustomVectorSearchClient;
        private readonly SearchClient _combinedSearchClient;
        private readonly SearchClient _securitySearchClient;
        private readonly SearchClient _aiEnrichImageSearchClient;
        private readonly SearchClient _aiEnrichCustomEntityLookupSearchClient;
        private readonly SearchClient _aiEnrichEntityLinkingSearchClient;
        private readonly SearchClient _aiEnrichEntityRecognitionSearchClient;
        private readonly SearchClient _aiEnrichKeyPhraseExtractionSearchClient;
        private readonly SearchClient _aiEnrichLanguageDetectionSearchClient;
        private readonly SearchClient _aiEnrichPIIDetectionSearchClient;
        private readonly SearchServiceConfig _config;

        /// <summary>
        /// SearchService
        /// </summary>
        /// <param name="options"></param>
        public SearchService(IOptions<SearchServiceConfig> options)
        {
            _config = options.Value;

            // Assuming Managed Identity is configured correctly in the Azure service.
            _keywordSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameKeyword,
                new DefaultAzureCredential());

            _vectorSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameVector,
                new DefaultAzureCredential());

            _hybridSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameVectorSemantic,
                new DefaultAzureCredential());

            _combinedSearchClient = new SearchClient(new Uri($"https://{options.Value.ServiceName}.search.windows.net/"),
                _config.IndexNameCombined,
                new DefaultAzureCredential());

            _securitySearchClient = new SearchClient(new Uri($"https://{options.Value.ServiceName}.search.windows.net/"),
                _config.IndexNameSecurity,
                new DefaultAzureCredential());

            _hybridCustomVectorSearchClient = new SearchClient(new Uri($"https://{options.Value.ServiceName}.search.windows.net/"),
                _config.IndexNameHybridVectorAzFunc,
                new DefaultAzureCredential());

            _aiEnrichImageSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichImage,
                new DefaultAzureCredential());

            _aiEnrichCustomEntityLookupSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichCustomEntityLookup,
                new DefaultAzureCredential());

            _aiEnrichEntityLinkingSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichEntityLinking,
                new DefaultAzureCredential());

            _aiEnrichEntityRecognitionSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichEntityRecognition,
                new DefaultAzureCredential());

            _aiEnrichKeyPhraseExtractionSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichKeyPhraseExtraction,
                new DefaultAzureCredential());

            _aiEnrichLanguageDetectionSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichLanguageDetection,
                new DefaultAzureCredential());

            _aiEnrichPIIDetectionSearchClient = new SearchClient(new Uri($"https://{_config.ServiceName}.search.windows.net/"),
                _config.IndexNameAIEnrichPIIDetection,
                new DefaultAzureCredential());
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
                model.HybridAdaSearchResults = await HybridSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Hybrid-CustomVector"))
            {
                model.HybridCustomVectorSearchResults = await HybridCustomVectorSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("SQL+Normal"))
            {
                model.CombinedSearchResults = await CombinedSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Document Security"))
            {
                model.SecuritySearchResults = await SecuritySearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Image"))
            {
                model.AIEnrichImageSearchResults = await AIEnrichImageSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Custom Entity Lookup"))
            {
                model.AIEnrichCustomEntityLookupSearchResults = await AIEnrichCustomEntityLookupSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Entity Linking"))
            {
                model.AIEnrichEntityLinkingSearchResults = await AIEnrichEntityLinkingSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Entity Recognition"))
            {
                model.AIEnrichEntityRecognitionSearchResults = await AIEnrichEntityRecognitionSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Key Phrase Extraction"))
            {
                model.AIEnrichKeyPhraseExtractionSearchResults = await AIEnrichKeyPhraseExtractionSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Language Detection"))
            {
                model.AIEnrichLanguageDetectionSearchResults = await AIEnrichLanguageDetectionSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - PII Detection"))
            {
                model.AIEnrichPIIDetectionSearchResults = await AIEnrichPIIDetectionSearchAsync(model);
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

            if (model.SearchQuery !="*")
            {
                if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
                {
                    options.HighlightFields.Add($"chunk-{model.CountHighlightResult}");
                }
                else
                {
                    options.HighlightFields.Add("chunk");
                }
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
        /// HybridCustomVectorSearchAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResults</returns>
        public async Task<SearchResults<SearchDocument>> HybridCustomVectorSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("chunk");
            options.Select.Add("title");

            if (model.SearchQuery != "*")
            {
                if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
                {
                    options.HighlightFields.Add($"chunk-{model.CountHighlightResult}");
                }
                else
                {
                    options.HighlightFields.Add("chunk");
                }
            }

            options.QueryType = SearchQueryType.Semantic;
            options.QueryLanguage = QueryLanguage.JaJp;
            options.SemanticSearch = new()
            {
                SemanticConfigurationName = _config.SemanticConfigurationVectorAzFuncName,
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

            return await _hybridCustomVectorSearchClient.SearchAsync<SearchDocument>(model?.SearchQuery, options);
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

        public async Task<SearchResults<SearchDocument>> SecuritySearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("UserGroup");
            options.Filter = "UserGroup/any(g:search.in(g, 'mtamizharasa@microsoft.com, clement.lucas@microsoft.com'))";

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _securitySearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichImageSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("text");
            options.Select.Add("layoutText");
            options.Select.Add("imageTags");
            options.Select.Add("imageCaption");
            options.Select.Add("merged_content");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"merged_content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("merged_content");
            }

            return await _aiEnrichImageSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichCustomEntityLookupSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("matchedEntities");            

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _aiEnrichCustomEntityLookupSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichEntityLinkingSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("linkedEntities");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _aiEnrichEntityLinkingSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichEntityRecognitionSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("persons");
            options.Select.Add("locations");
            options.Select.Add("organizations");
            options.Select.Add("quantities");
            options.Select.Add("dateTimes");
            options.Select.Add("urls");
            options.Select.Add("emails");
            options.Select.Add("personTypes");
            options.Select.Add("events");
            options.Select.Add("products");
            options.Select.Add("skills");
            options.Select.Add("addresses");
            options.Select.Add("phoneNumbers");
            options.Select.Add("ipAddresses");
            options.Select.Add("namedEntities");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _aiEnrichEntityRecognitionSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichKeyPhraseExtractionSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("keyPhrases");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _aiEnrichKeyPhraseExtractionSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichLanguageDetectionSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("languageCode");
            options.Select.Add("languageName");
            options.Select.Add("languageScore");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _aiEnrichLanguageDetectionSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichPIIDetectionSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("piiEntities");
            options.Select.Add("maskedText");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _aiEnrichPIIDetectionSearchClient.SearchAsync<SearchDocument>(model.SearchQuery, options);
        }
    }
}
