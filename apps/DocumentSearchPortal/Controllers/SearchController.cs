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
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Results(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("Index");
            }

            // Perform the search    
            SearchResults<SearchDocument> results = await _searchService.SearchAsync(query);

            // Create the view model and populate the SearchResults property    
            var viewModel = new SearchResultViewModel
            {
                SearchResults = results
            };

            // Pass the view model to the view    
            return View(viewModel);
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
