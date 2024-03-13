using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using DocumentSearchPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            var viewModel = GetSearchResultViewModelFromTempData();
            return View(viewModel);
        }

        private SearchResultViewModel GetSearchResultViewModelFromTempData()
        {
            return new SearchResultViewModel
            {
                SearchQuery = TempData["Query"] as string,
                TopAISearch = TempData["TopAISearch"] as string,
                TopPortal = TempData["TopPortal"] as string,
                FilterExpression = TempData["FilterExpression"] as string,
                OrderByExpression = TempData["OrderByExpression"] as string,
                SearchMode = TempData["SearchMode"] as string                

            };
        }


        [HttpPost]
        public async Task<IActionResult> Search(SearchResultViewModel model)
        {

            model.NormalSearchResults = await _searchService.KeywordSearchAsync(model);
            model.VectorSearchResults = await _searchService.VectorSearchAsync(model);
            model.HybridSearchResults = await _searchService.HybridSearchAsync(model);

            //Store additional search parameters in TempData
            TempData["Query"] = model.SearchQuery;
            TempData["FilterExpression"] = model.FilterExpression;
            TempData["OrderByExpression"] = model.OrderByExpression;
            TempData["TopAISearch"] = model.TopAISearch;
            TempData["TopPortal"] = model.TopPortal;
            TempData["SearchMode"] = model.SearchMode;

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

        //public class ErrorViewModel
        //{
        //    public string? RequestId { get; set; } 
        //}
    }
}
