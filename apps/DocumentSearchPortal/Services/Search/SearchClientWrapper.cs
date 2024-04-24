using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DocumentSearchPortal.Services.Search
{
    public class SearchClientWrapper : ISearchClientWrapper
    {
        private readonly Uri _serviceEndpoint;
        private readonly AzureKeyCredential _credential;
        private readonly TokenCredential _tokenCredential;

        public SearchClientWrapper(IOptions<SearchServiceConfig> config)
        {
            // Retrieve values from configuration  
            string serviceName = config.Value.ServiceName;
            string apiKey = "";

            _serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");

            // Check if apiKey has a non-default value  
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _credential = new AzureKeyCredential(apiKey);
            }
            else
            {
                _tokenCredential = new DefaultAzureCredential();
            }
        }

        public async Task<SearchResults<SearchDocument>> SearchAsync(string searchQuery, SearchOptions options, string indexName)
        {
            SearchClient client;

            // Initialize the SearchClient with either the AzureKeyCredential or DefaultAzureCredential  
            if (_credential != null)
            {
                client = new SearchClient(_serviceEndpoint, indexName, _credential);
            }
            else
            {
                client = new SearchClient(_serviceEndpoint, indexName, _tokenCredential);
            }

            return await client.SearchAsync<SearchDocument>(searchQuery, options);
        }
    }
}
