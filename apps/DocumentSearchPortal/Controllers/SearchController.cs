using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using DocumentSearchPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            var viewModel = new SearchResultViewModel
            {
                NormalSearchResults = TempData["NormalResults"] as SearchResults<SearchDocument>,
                SearchQuery = TempData["Query"] as string,
                VectorSearchResults = TempData["VectorResults"] as SearchResults<SearchDocument>,                
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchResultViewModel model)
        {

            model.NormalSearchResults = await _keywordSearchService.KeywordSearchAsync(model);
            model.VectorSearchResults = await _vectorSearchService.VectorSearchAsync(model);
            return View("Index", model); // Pass the view model to the Index view directly  
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
