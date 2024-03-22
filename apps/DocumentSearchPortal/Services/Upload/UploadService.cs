using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Storage.Blobs;
using DocumentSearchPortal.Data;
using DocumentSearchPortal.Models;
using DocumentSearchPortal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using UglyToad.PdfPig;

namespace DocumentSearchPortal.Services.Upload
{
    /// <summary>
    /// UploadService
    /// </summary>
    public class UploadService : IUploadService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly ApplicationDbContext _context;
        private readonly SearchClient _combinedSearchClient;
        private readonly SearchIndexerClient _combinedSearchIndexerClient;
        private readonly string _combinedIndexer1Name;
        private readonly string _combinedIndexer2Name;

        /// <summary>
        /// UploadService
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        public UploadService(IOptions<SearchServiceConfig> options, ApplicationDbContext context) // Inject ApplicationDbContext  
        {
            _connectionString = options.Value.AzureBlobStorageConnectionString;
            _containerName = options.Value.UploadContainer;
            _combinedIndexer1Name = options.Value.CombinedIndexer1Name;
            _combinedIndexer2Name = options.Value.CombinedIndexer2Name;
            _context = context;

            // Assuming Managed Identity is configured correctly in the Azure service.
            _combinedSearchClient = new SearchClient(new Uri($"https://{options.Value.ServiceName}.search.windows.net/"), options.Value.CombinedIndexName, new DefaultAzureCredential());
            _combinedSearchIndexerClient = new SearchIndexerClient(new Uri($"https://{options.Value.ServiceName}.search.windows.net/"), new DefaultAzureCredential());
        }

        /// <summary>
        /// PrepareIndexViewModelAsync
        /// </summary>
        /// <returns>UploadViewModel</returns>
        public async Task<UploadViewModel> PrepareIndexViewModelAsync()
        {
            var categories = await _context.Categories
                .Select(c => new { c.CategoryId, c.Name })
                .ToListAsync();

            var protocols = await _context.Protocols
                .Select(p => new { p.ProtocolId, p.Name })
                .ToListAsync();

            var disclosureScopes = await _context.DisclosureScopes
                .Select(d => new SelectListItem { Value = d.DisclosureScopeId.ToString(), Text = d.Name })
                .ToListAsync();

            var informationIds = await _context.Information
                .Select(i => new { i.InformationId, i.Name })
                .ToListAsync();

            return new UploadViewModel
            {
                Categories = new SelectList(categories, "CategoryId", "Name"),
                Protocols = new SelectList(protocols, "ProtocolId", "Name"),
                DisclosureScopes = disclosureScopes,
                InformationIds = new SelectList(informationIds, "InformationId", "Name"),
            };
        }

        /// <summary>
        /// UploadDocumentsAsync
        /// </summary>
        /// <param name="files"></param>
        /// <param name="uploadViewModel"></param>
        /// <param name="newInformationName"></param>
        /// <returns>IEnumerable<UploadResult></returns>
        public async Task<IEnumerable<UploadResult>> UploadDocumentsAsync(IList<IFormFile> files, UploadViewModel uploadViewModel, string newInformationName = null)
        {
            List<UploadResult> uploadResults = new List<UploadResult>();
            int selectedInformationId = 0;

            // Add the New Information if exists
            if (!string.IsNullOrEmpty(newInformationName))
            {
                var existingInformation = await _context.Information.FirstOrDefaultAsync(i => i.Name == newInformationName);
                if (existingInformation == null)
                {
                    var newInformation = new Information { Name = newInformationName };
                    await _context.Information.AddAsync(newInformation);
                    await _context.SaveChangesAsync();
                    selectedInformationId = newInformation.InformationId;
                }
                else
                {
                    selectedInformationId = existingInformation.InformationId;
                }
            }
            else
            {
                selectedInformationId = uploadViewModel.SelectedInformationId;
            }

            // Get the Protocol Name and Information Name to frame the Virtual Container folder path
            string? selectedProtocolName = await _context.Protocols
            .Where(c => c.ProtocolId == uploadViewModel.SelectedProtocolId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync();

            string? selectedInformationName = await _context.Information
            .Where(c => c.InformationId == selectedInformationId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync();            

            // Loop the files and save
            foreach (IFormFile file in files)
            {
                try
                {
                    // Construct the blob name by combining the virtual directory path and the file name    
                    string blobName = selectedProtocolName + "/" + selectedInformationName + "/" + file.FileName;

                    // Upload the Document to Azure Blob
                    string blobUri = await UploadFileToBlobAsync(blobName, file);

                    Models.Document documentMetadata = new Models.Document();

                    // Populate the common metadata for all the files.
                    documentMetadata.CategoryId = uploadViewModel.SelectedCategoryId;
                    documentMetadata.InformationId = selectedInformationId;
                    documentMetadata.ProtocolId = uploadViewModel.SelectedProtocolId;
                    documentMetadata.DisclosureScopeId = uploadViewModel.SelectedDisclosureScopeId;
                    documentMetadata.Description = uploadViewModel.Description;

                    //For now, hardcoded this. TODO: This should be replaced with actual user ID
                    documentMetadata.CreatedById = 1;
                    documentMetadata.ModifiedById = 1;

                    // Populate the file specific metadata for the file.
                    documentMetadata.metadata_storage_path = blobUri;
                    documentMetadata.FileName = file.FileName;

                    // Save metadata to database
                    await _context.Documents.AddAsync(documentMetadata);
                    await _context.SaveChangesAsync();

                    int documentId = documentMetadata.DocumentId;                   

                    // Option-1: Update the search index manually (Least preferred for this scenario)
                    //string? extractedText = await ExtractTextFromPdfAsync(file); // If PDF
                    //await UpdateSearchIndexAsync(documentId, extractedText);

                    uploadResults.Add(new UploadResult
                    {
                        Success = true,
                        Message = "File uploaded successfully",
                        Uri = blobUri
                    });
                }
                catch (Exception ex)
                {
                    uploadResults.Add(new UploadResult
                    {
                        Success = false,
                        Message = $"Error uploading file: {ex.Message}"
                    });
                }
            }

            if (uploadResults.Where(c => c.Success == true).Count() > 0)
            {
                // Option-2: Run the Indexer
                await _combinedSearchIndexerClient.RunIndexerAsync(_combinedIndexer1Name);
                await _combinedSearchIndexerClient.RunIndexerAsync(_combinedIndexer2Name);
            }
            
            return uploadResults;
        }

        /// <summary>
        /// UploadFileToBlobAsync
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="file"></param>
        /// <returns>string</returns>
        private async Task<string> UploadFileToBlobAsync(string blobName, IFormFile file)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            await using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }

        /// <summary>
        /// ExtractTextFromPdfAsync
        /// </summary>
        /// <param name="file"></param>
        /// <returns>string</returns>
        private async Task<string> ExtractTextFromPdfAsync(IFormFile file)
        {
            string content = string.Empty;

            if (file.ContentType == "application/pdf")
            {
                using (var stream = file.OpenReadStream())
                using (var pdf = PdfDocument.Open(stream))
                {
                    var textBuilder = new System.Text.StringBuilder();

                    foreach (var page in pdf.GetPages())
                    {
                        textBuilder.AppendLine(page.Text);
                    }

                    content = textBuilder.ToString();
                }
            }

            return content;
        }

        /// <summary>
        /// UpdateSearchIndexAsync
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="extractedText"></param>
        /// <returns></returns>
        private async Task UpdateSearchIndexAsync(int documentId, string? extractedText)
        {
            var doc = await _context.vw_DocumentsDetails
            .Where(c => Convert.ToInt32(c.DocumentId) == documentId)
            .FirstOrDefaultAsync();

            var indexDocument = new
            {
                DocumentId = doc?.DocumentId.ToString(),
                FileName = doc?.FileName,
                Description = doc?.Description,
                Version = doc?.Version,
                CategoryName = doc?.CategoryName,
                ProtocolName = doc?.ProtocolName,
                CreatedByName = doc?.CreatedByName,
                ModifiedByName = doc?.ModifiedByName,
                DisclosureScopeName = doc?.DisclosureScopeName,
                InformationName = doc?.InformationName,
                metadata_storage_name = doc?.FileName,
                metadata_storage_path = Convert.ToBase64String(Encoding.UTF8.GetBytes(doc?.metadata_storage_path)),
                metadata_storage_last_modified = DateTime.Now,
                content = extractedText,
            };
            await _combinedSearchClient.MergeOrUploadDocumentsAsync(new[] { indexDocument });
        }
    }

    /// <summary>
    /// UploadResult
    /// </summary>
    public class UploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Uri { get; set; }
    }
}
