using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FujitsuAzureAISearch
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string serviceName = ""; // search-service-name
            string indexName = ""; // index-name
            string apiKey = ""; // query-api-key"
            string searchText = "名前";

            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(apiKey);

            SearchClient searchClient = new SearchClient(serviceEndpoint, indexName, credential);

            SearchResults<SearchDocument> searchResults = await searchClient.SearchAsync<SearchDocument>(searchText);

            foreach (SearchResult<SearchDocument> result in searchResults.GetResults())
            {
                // Output the search results  
                Console.WriteLine(result.Document["content"]);
            }

            Console.ReadLine();
        }
    }
}
