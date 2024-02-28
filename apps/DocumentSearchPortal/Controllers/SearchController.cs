using DocumentSearchPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Services;

namespace DocumentSearchPortal.Controllers
{
    public class SearchController : Controller
    {
        private readonly KeywordSearchService _keywordSearchService;
        private readonly VectorSearchService _vectorSearchService;

        public SearchController(KeywordSearchService keywordSearchService, VectorSearchService vectorSearchService)
        {
            _keywordSearchService = keywordSearchService;
            _vectorSearchService = vectorSearchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> NormalSearchResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("Index");
            }

            // Perform the normal index search  
            SearchResults<SearchDocument> normalResults = await _keywordSearchService.KeywordSearchAsync(query);

            // Create the view model and populate the SearchResults property  
            var normalViewModel = new SearchResultViewModel
            {
                SearchResults = normalResults
            };

            // Pass the view model to the view  
            return View("Results", normalViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> VectorSearchResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("Index");
            }

            // Perform the vector search  
            SearchResults<SearchDocument> vectorResults = await _vectorSearchService.VectorSearchAsync(query);

            // Create the view model and populate the SearchResults property  
            var vectorViewModel = new SearchResultViewModel
            {
                SearchResults = vectorResults
            };

            // Pass the view model to the view  
            return View("Results", vectorViewModel);
        }

        // Action to display a custom error page  
        [HttpGet]
        public IActionResult Error()
        {
            // Retrieve error details if needed (e.g., for logging)  
            var exceptionDetails = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            // Log the exception details or handle them as needed  
            // _logger.LogError(exceptionDetails.Error, "An error occurred.");  

            // Prepare a view model if needed to pass additional data to the error view  
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier  
            };

            // Return the Error view with the view model  
            return View(errorViewModel);
        }

        public class ErrorViewModel
        {
            public string? RequestId { get; set; } 
        }
    }
}
