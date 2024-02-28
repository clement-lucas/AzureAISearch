using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using FujitsuAzureAISearch.Models;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;
using Azure;

namespace FujitsuAzureAISearch
{
    internal class Program
    {
        private static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
        {
            // Build the configuration and read settings from appsettings.json  
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Retrieve the storage configuration  
            StorageConfiguration storageConfig = Configuration.GetSection("StorageAccount").Get<StorageConfiguration>();
            AISearchConfiguration aiSearchConfig = Configuration.GetSection("AISearchService").Get<AISearchConfiguration>();

            // Call the method to clear the container  
            await ClearContainerAsync(storageConfig);

            // Call the method to upload files from the specified folder  
            await UploadFilesFromFolder(storageConfig);

            // Call the method to create index
            await CreateSearchIndexAsync(aiSearchConfig);

            // Call the method to create indexer
            await CreateBlobIndexerAsync(storageConfig, aiSearchConfig);


            Console.ReadLine();
        }

        private static async Task ClearContainerAsync(StorageConfiguration storageConfig)
        {
            // Create the container client object  
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.ContainerName);

            // Ensure that the container exists  
            if (await containerClient.ExistsAsync())
            {
                // List all blobs in the container and delete each one  
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    // Get a reference to the blob client  
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

                    // Delete the blob  
                    await blobClient.DeleteIfExistsAsync();
                    Console.WriteLine($"Deleted blob: {blobItem.Name}");
                }
            }
            else
            {
                Console.WriteLine("The specified container does not exist.");
            }
        }

        private static async Task UploadFilesFromFolder(StorageConfiguration storageConfig)
        {
            // Retrieve the 'Sample Files' folder path from the configuration    
            string relativeFolderPath = Configuration["LocalFolderPath"];

            // Combine the base directory with the relative path to get the absolute path    
            string localFolderPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\" + relativeFolderPath);

            // Create the container client object    
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.ContainerName);

            // Ensure that the container exists. Create a container if doesn't exist.   
            await containerClient.CreateIfNotExistsAsync();

            // Get all files from the folder    
            string[] fileEntries = Directory.GetFiles(localFolderPath);
            foreach (string filePath in fileEntries)
            {
                string fileName = Path.GetFileName(filePath);

                // Construct the blob name by combining the virtual directory and the file name    
                string blobName = storageConfig.VirtualDirectory.TrimEnd('/') + "/" + fileName;

                // Get a reference to the blob client  
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Open the file and upload its data    
                using (FileStream uploadFileStream = File.OpenRead(filePath))
                {
                    // Define the metadata for the blob    
                    Dictionary<string, string> metadata = new Dictionary<string, string>
                    {
                        {"Category", "Category1"},
                        {"InformationId", "Information1"},
                        {"ProtocolId", "Protocol1"},
                        {"DisclosureScope", "Public"},
                        {"ModifiedBy", "Manivelarasan Tamizharasan"},
                        {"Description", "Sample file uploaded with metadata. File path: " + blobName}
                    };

                    try
                    {
                        // Upload the file with metadata    
                        await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders { ContentType = "application/pdf" }); // Changed ContentType to 'application/pdf'  
                        await blobClient.SetMetadataAsync(metadata);

                        Console.WriteLine($"Uploaded {fileName} successfully");
                    }
                    catch (Azure.RequestFailedException ex)
                    {
                        Console.WriteLine($"Error uploading file {fileName}: {ex.Message}");
                        Console.WriteLine($"Status: {ex.Status}");
                        Console.WriteLine($"ErrorCode: {ex.ErrorCode}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                        }
                    }
                }
            }
        }

        public static async Task CreateSearchIndexAsync(AISearchConfiguration aiSearchConfig)
        {
            Uri serviceEndpoint = new Uri($"https://{aiSearchConfig.ServiceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(aiSearchConfig.ApiKey);

            SearchIndexClient indexClient = new SearchIndexClient(serviceEndpoint, credential);

            FieldBuilder fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(DocumentMetadata));

            SearchIndex searchIndex = new SearchIndex(aiSearchConfig.IndexName, searchFields);

            // Customize the index if needed, for example, setting up scoring profiles, suggesters, analyzers, etc.

            // If we have run the sample before, this index will be populated
            // We can clear the index by deleting it if it exists and creating
            // it again
            CleanupSearchIndexClientResources(indexClient, searchIndex);

            await indexClient.CreateOrUpdateIndexAsync(searchIndex);
        }

        public class DocumentMetadata
        {
            [SimpleField(IsKey = true, IsFilterable = true)]
            public string FileName { get; set; }

            [SearchableField(IsFilterable = true, IsSortable = true)]
            public string Category { get; set; }

            [SearchableField(IsFilterable = true, IsSortable = true)]
            public string InformationId { get; set; }

            [SearchableField(IsFilterable = true, IsSortable = true)]
            public string ProtocolId { get; set; }

            [SearchableField(IsFilterable = true, IsSortable = true)]
            public string DisclosureScope { get; set; }

            [SearchableField(IsFilterable = true, IsSortable = true)]
            public string ModifiedBy { get; set; }

            [SearchableField(IsSortable = true)]
            public string Description { get; set; }
        }
        public static async Task CreateBlobIndexerAsync(StorageConfiguration storageConfig, AISearchConfiguration aiSearchConfig)
        {
            Uri serviceEndpoint = new Uri($"https://{aiSearchConfig.ServiceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(aiSearchConfig.ApiKey);

            SearchIndexerClient indexerClient = new SearchIndexerClient(serviceEndpoint, credential);

            // Create or update the data source connection  
            SearchIndexerDataSourceConnection dataSource = new SearchIndexerDataSourceConnection(
                name: aiSearchConfig.DataSourceName,
                type: SearchIndexerDataSourceType.AzureBlob,
                connectionString: storageConfig.ConnectionString,
                container: new SearchIndexerDataContainer(name: storageConfig.ContainerName)
            );

            await indexerClient.CreateOrUpdateDataSourceConnectionAsync(dataSource);

            var schedule = new IndexingSchedule(TimeSpan.FromDays(1)) // How often to run the indexer  
            {
                StartTime = DateTimeOffset.Now
            };

            var parameters = new IndexingParameters()
            {
                BatchSize = 100,
                MaxFailedItems = 0,
                MaxFailedItemsPerBatch = 0
            };

            // Create an indexer with the appropriate parameters  
            SearchIndexer indexer = new SearchIndexer(name: $"{aiSearchConfig.IndexName}-indexer", dataSourceName: aiSearchConfig.DataSourceName, targetIndexName: aiSearchConfig.IndexName)
            {
                Description = "File Indexer",
                Schedule = schedule,
                Parameters = parameters,
                FieldMappings =
                {
                    new FieldMapping("metadata_storage_name") { TargetFieldName = "FileName" },
                    new FieldMapping("Category") { TargetFieldName = "Category" },
                    new FieldMapping("InformationId") { TargetFieldName = "InformationId" },
                    new FieldMapping("ProtocolId") { TargetFieldName = "ProtocolId" },
                    new FieldMapping("DisclosureScope") { TargetFieldName = "DisclosureScope" },
                    new FieldMapping("ModifiedBy") { TargetFieldName = "ModifiedBy" },
                    new FieldMapping("Description") { TargetFieldName = "Description" }  
                    // Add other field mappings as necessary  
                } 
            };

            // Indexers contain metadata about how much they have already indexed
            // If we already ran the sample, the indexer will remember that it already
            // indexed the sample data and not run again
            // To avoid this, reset the indexer if it exists
            CleanupSearchIndexerClientResources(indexerClient, indexer);

            await indexerClient.CreateOrUpdateIndexerAsync(indexer);

            // We created the indexer with a schedule, but we also
            // want to run it immediately
            Console.WriteLine("Running Azure AI Search indexer...");

            try
            {
                await indexerClient.RunIndexerAsync(indexer.Name);
            }
            catch (RequestFailedException ex) when (ex.Status == 429)
            {
                Console.WriteLine("Failed to run indexer: {0}", ex.Message);
            }

            // Wait 5 seconds for indexing to complete before checking status
            Console.WriteLine("Waiting for indexing...\n");
            System.Threading.Thread.Sleep(5000);

            // After an indexer run, you can retrieve status.
            CheckIndexerStatus(aiSearchConfig.IndexName, indexerClient, indexer);
        }

        private static void CheckIndexerStatus(string indexName, SearchIndexerClient indexerClient, SearchIndexer indexer)
        {
            try
            {
                string indexerName = $"{indexName}-indexer";
                SearchIndexerStatus execInfo = indexerClient.GetIndexerStatus(indexerName);

                Console.WriteLine("Indexer has run {0} times.", execInfo.ExecutionHistory.Count);
                Console.WriteLine("Indexer Status: " + execInfo.Status.ToString());

                IndexerExecutionResult result = execInfo.LastResult;

                Console.WriteLine("Latest run");
                Console.WriteLine("Run Status: {0}", result.Status.ToString());
                Console.WriteLine("Total Documents: {0}, Failed: {1}", result.ItemCount, result.FailedItemCount);

                //TimeSpan elapsed = result.EndTime.Value - result.StartTime.Value;
                //Console.WriteLine("StartTime: {0:T}, EndTime: {1:T}, Elapsed: {2:t}", result.StartTime.Value, result.EndTime.Value, elapsed);

                string errorMsg = (result.ErrorMessage == null) ? "none" : result.ErrorMessage;
                Console.WriteLine("ErrorMessage: {0}", errorMsg);
                Console.WriteLine(" Document Errors: {0}, Warnings: {1}\n", result.Errors.Count, result.Warnings.Count);
            }
            catch (RequestFailedException ex) when (ex.Status == 429)
            {
                Console.WriteLine("Failed to run indexer check: {0}", ex.Message);
            }
        }

        private static void CleanupSearchIndexClientResources(SearchIndexClient indexClient, SearchIndex searchIndex)
        {
            try
            {
                if (indexClient.GetIndex(searchIndex.Name) != null)
                {
                    indexClient.DeleteIndex(searchIndex.Name);
                }
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                //if exception occurred and status is "Not Found", this is working as expected
                Console.WriteLine("If an index of the same name is detected, it's deleted now so that we can reuse the name.");
            }
        }

        private static void CleanupSearchIndexerClientResources(SearchIndexerClient indexerClient, SearchIndexer indexer)
        {
            try
            {
                if (indexerClient.GetIndexer(indexer.Name) != null)
                {
                    indexerClient.ResetIndexer(indexer.Name);
                }
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                //if exception occurred and status is "Not Found", this is working as expected
                Console.WriteLine("The indexer doesn't exist.");
            }
        }

        private static async Task SampleSearch()
        {
            string serviceName = ""; // search-service-name
            string indexName = ""; // index-name
            string apiKey = ""; // query-api-key"
            string searchText = "名前";

            Uri serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            Azure.AzureKeyCredential credential = new Azure.AzureKeyCredential(apiKey);

            SearchClient searchClient = new SearchClient(serviceEndpoint, indexName, credential);

            SearchResults<SearchDocument> searchResults = await searchClient.SearchAsync<SearchDocument>(searchText);

            foreach (SearchResult<SearchDocument> result in searchResults.GetResults())
            {
                // Output the search results  
                Console.WriteLine(result.Document["content"]);
            }
        }
    }
}
