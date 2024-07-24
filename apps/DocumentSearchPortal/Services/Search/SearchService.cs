using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using Microsoft.Extensions.Options;
using System.Text;


namespace DocumentSearchPortal.Services.Search
{
    /// <summary>
    /// SearchService
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly ISearchClientWrapper _searchClientWrapper;
        private readonly SearchServiceConfig _config;

        /// <summary>
        /// SearchService
        /// </summary>
        /// <param name="options"></param>
        public SearchService(IOptions<SearchServiceConfig> options, ISearchClientWrapper searchClientWrapper)
        {
            _config = options.Value;
            _searchClientWrapper = searchClientWrapper;
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

            if (model.SelectedIndexes.Contains("Normal - Japanese Analyzer"))
            {
                model.NormalJapaneseSearchResults = await KeywordJpSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Normal - English Analyzer"))
            {
                model.NormalEnglishSearchResults = await KeywordEngSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Normal - Standard Analyzer"))
            {
                model.NormalStandardSearchResults = await KeywordStdSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Normal - JP+EN"))
            {
                model.MultiLanguageSearchResults = await MultiLanguageSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Normal - JP+EN+STDlucene")) 
            {
                model.MultiLanguageAndStdSearchResults = await MultiLanguageAndStdSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Vector"))
            {
                model.VectorSearchResults = await VectorSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("Hybrid"))
            {
                model.HybridAdaSearchResults = await HybridSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("New Hybrid"))
            {
                model.NewHybridSearchResults = await NewHybridSearchAsync(model);
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

            if (model.SelectedIndexes.Contains("AI Enrichment - Text Translation"))
            {
                model.AIEnrichTranslationSearchResults = await AIEnrichTranslationSearchAsync(model);
            }

            if (model.SelectedIndexes.Contains("AI Enrichment - Sentiment"))
            {
                model.AIEnrichSentimentSearchResults = await AIEnrichSentimentSearchAsync(model);
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
        public async Task<SearchResults<SearchDocument>> KeywordJpSearchAsync(SearchResultViewModel model)
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameKeywordJapanese);
        }

        public async Task<SearchResults<SearchDocument>> KeywordEngSearchAsync(SearchResultViewModel model)
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameKeywordEnglish);
        }

        public async Task<SearchResults<SearchDocument>> KeywordStdSearchAsync(SearchResultViewModel model)
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameKeywordStandard);
        }

        public async Task<SearchResults<SearchDocument>> MultiLanguageSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("content_en");
            options.Select.Add("metadata_storage_name");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
                options.HighlightFields.Add($"content_en-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
                options.HighlightFields.Add("content_en");
            }

            SearchResults<SearchDocument> searchResults = await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameMultiLanguage);

            foreach (SearchResult<SearchDocument> result in searchResults.GetResults())
            {
                // Check if both 'content' and 'content_en' highlights exist  
                if (result?.Highlights?.Count > 0 && result.Highlights.ContainsKey("content") && result.Highlights.ContainsKey("content_en"))
                {
                    // Get 'content' highlights as a HashSet for efficient lookups  
                    HashSet<string>? contentHighlightsSet = new HashSet<string>(result.Highlights["content"]);

                    // Filter 'content_en' highlights, removing any that exist in 'content' highlights  
                    List<string>? filteredContentEnHighlights = result.Highlights["content_en"]
                        .Where(highlight => !contentHighlightsSet.Contains(highlight))
                        .ToList();

                    // Update 'content_en' highlights with the filtered list  
                    result.Highlights["content_en"] = filteredContentEnHighlights;
                }
            }

            return searchResults;
        }

        public async Task<SearchResults<SearchDocument>> MultiLanguageAndStdSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("content_en");
            options.Select.Add("content_std");
            options.Select.Add("metadata_storage_name");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
                options.HighlightFields.Add($"content_en-{model.CountHighlightResult}");
                options.HighlightFields.Add($"content_std-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
                options.HighlightFields.Add("content_en");
                options.HighlightFields.Add("content_std");
            }

            SearchResults<SearchDocument> searchResults = await _searchClientWrapper.SearchAsync(model.SearchQuery, options, "index-multilanguageandstd-fju-nonprod-jpeast-01");

            foreach (SearchResult<SearchDocument> result in searchResults.GetResults())
            {
                if (result?.Highlights?.Count > 0)
                {
                    // Store non-merged highlights  
                    //result.Document["highlights_content_jp"] = result.Highlights.ContainsKey("content") ? result.Highlights["content"] : new List<string>();
                    result.Document["highlights_content_en"] = result.Highlights.ContainsKey("content_en") ? result.Highlights["content_en"] : new List<string>();
                    result.Document["highlights_content_std"] = result.Highlights.ContainsKey("content_std") ? result.Highlights["content_std"] : new List<string>();

                    // Merge all highlights from different analyzers  
                    HashSet<string> uniqueHighlights = new HashSet<string>();

                    if (result.Highlights.ContainsKey("content"))
                    {
                        uniqueHighlights.UnionWith(result.Highlights["content"]);
                    }
                    if (result.Highlights.ContainsKey("content_en"))
                    {
                        uniqueHighlights.UnionWith(result.Highlights["content_en"]);
                    }
                    if (result.Highlights.ContainsKey("content_std"))
                    {
                        uniqueHighlights.UnionWith(result.Highlights["content_std"]);
                    }

                    // Merge highlights into a list of merged sentences  
                    List<string> mergedHighlights = MergeHighlightTexts(uniqueHighlights.ToList());

                    // Store merged highlights  
                    result.Document["merged_highlights"] = mergedHighlights;
                }
            }

            return searchResults;
        }

        private List<string> MergeHighlightTexts(List<string> highlights)
        {
            if (highlights == null || highlights.Count == 0)
                return new List<string>();

            var uniqueSentences = new Dictionary<string, List<(int Start, int End)>>();

            foreach (var highlight in highlights)
            {
                var normalizedSentence = new StringBuilder();
                var tagPositions = new List<(int Start, int End)>();

                int offset = 0;
                while (offset < highlight.Length)
                {
                    int start = highlight.IndexOf("<em>", offset);
                    int end = highlight.IndexOf("</em>", offset);

                    if (start >= 0 && (start < end || end == -1))
                    {
                        normalizedSentence.Append(highlight, offset, start - offset);
                        offset = start + "<em>".Length;
                        int textStart = normalizedSentence.Length;
                        end = highlight.IndexOf("</em>", offset);
                        if (end >= 0)
                        {
                            normalizedSentence.Append(highlight, offset, end - offset);
                            offset = end + "</em>".Length;
                            int textEnd = normalizedSentence.Length;
                            tagPositions.Add((textStart, textEnd));
                        }
                        else
                        {
                            normalizedSentence.Append(highlight, offset, highlight.Length - offset);
                            offset = highlight.Length;
                        }
                    }
                    else
                    {
                        normalizedSentence.Append(highlight, offset, highlight.Length - offset);
                        offset = highlight.Length;
                    }
                }

                var sentenceStr = normalizedSentence.ToString();

                if (!uniqueSentences.ContainsKey(sentenceStr))
                {
                    uniqueSentences[sentenceStr] = new List<(int Start, int End)>();
                }
                uniqueSentences[sentenceStr].AddRange(tagPositions);
            }

            var mergedSentences = new List<string>();

            foreach (var kvp in uniqueSentences)
            {
                var sentence = kvp.Key;
                var positions = kvp.Value.OrderBy(p => p.Start).ToList();

                var highlightedSentence = new StringBuilder();
                int lastIndex = 0;

                foreach (var (start, end) in positions)
                {
                    // Append text before highlight  
                    if (start > lastIndex)
                    {
                        highlightedSentence.Append(sentence, lastIndex, start - lastIndex);
                    }

                    // Append highlighted text  
                    highlightedSentence.Append("<em>");
                    highlightedSentence.Append(sentence, start, end - start);
                    highlightedSentence.Append("</em>");
                    lastIndex = end;
                }

                // Append remaining text after the last highlight  
                if (lastIndex < sentence.Length)
                {
                    highlightedSentence.Append(sentence, lastIndex, sentence.Length - lastIndex);
                }

                mergedSentences.Add(highlightedSentence.ToString());
            }

            // Remove any leading or trailing spaces to avoid accidental extra content  
            for (int i = 0; i < mergedSentences.Count; i++)
            {
                mergedSentences[i] = mergedSentences[i].Trim();
            }

            return mergedSentences;
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameVector);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameVectorSemantic);
        }

        /// <summary>
        /// NewHybridSearchAsync
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SearchResults<SearchDocument></returns>
        public async Task<SearchResults<SearchDocument>> NewHybridSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("chunk");
            options.Select.Add("title");
            options.SearchFields.Add("chunk");

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
                SemanticConfigurationName = _config.NewSemanticConfigurationName,
                QueryCaption = new(QueryCaptionType.Extractive),
                QueryAnswer = new(QueryAnswerType.None),
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameNewVectorSemantic);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameHybridVectorAzFunc);
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
            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameCombined);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameSecurity);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichImage);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichCustomEntityLookup);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichEntityLinking);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichEntityRecognition);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichKeyPhraseExtraction);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichLanguageDetection);
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

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichPIIDetection);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichTranslationSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("translatedText");
            options.Select.Add("translatedFromLanguageCode");
            options.Select.Add("translatedToLanguageCode");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichTranslation);
        }

        public async Task<SearchResults<SearchDocument>> AIEnrichSentimentSearchAsync(SearchResultViewModel model)
        {
            SearchOptions options = SetupCommonSearchOptions(model);
            options.Select.Add("content");
            options.Select.Add("metadata_storage_name");
            options.Select.Add("sentiment");
            options.Select.Add("sentences");
            options.Select.Add("confidenceScores");

            if (model.CountHighlightResult.HasValue && model.CountHighlightResult > 0)
            {
                options.HighlightFields.Add($"content-{model.CountHighlightResult}");
            }
            else
            {
                options.HighlightFields.Add("content");
            }

            return await _searchClientWrapper.SearchAsync(model.SearchQuery, options, _config.IndexNameAIEnrichSentiment);
        }
    }
}
