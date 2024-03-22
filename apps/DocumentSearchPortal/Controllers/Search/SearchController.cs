using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using DocumentSearchPortal.Services.Search;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace DocumentSearchPortal.Controllers.Search
{
    /// <summary>
    /// Controller for Search Page
    /// </summary>
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        /// <summary>
        /// Search Controller Constructor.
        /// </summary>
        /// <param name="searchService"></param>
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Landing function for Search Page.
        /// </summary>
        /// <returns>View of search page</returns>
        public IActionResult Index()
        {
            SearchResultViewModel viewModel = GetSearchResultViewModelFromTempData();
            return View(viewModel);

        }

        /// <summary>
        /// Gets temp data between page reloads.
        /// </summary>
        /// <returns>SearchResultViewModel</returns>
        private SearchResultViewModel GetSearchResultViewModelFromTempData()
        {
            // Load the selected values from Temp data.
            return new SearchResultViewModel
            {
                SearchQuery = TempData["SearchQuery"] as string,
                FilterExpression = TempData["FilterExpression"] as string,
                OrderByExpression = TempData["OrderByExpression"] as string,
                CountSearchResult = TempData["CountSearchResult"] as int?,
                CountHighlightResult = TempData["CountHighlightResult"] as int?,
                CountVectorResult = TempData["CountVectorResult"] as int?,
                CountPrefixSuffix = TempData["CountPrefixSuffix"] as int?,
                SearchMode = TempData["SearchMode"] as string,
                SelectedIndexes = TempData["SelectedIndexes"] as List<string>

            };
        }

        /// <summary>
        /// Function that gets called when search button is clicked
        /// </summary>
        /// <param name="model"></param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<IActionResult> Search(SearchResultViewModel model)
        {
            // Perform Search.
            if (!string.IsNullOrEmpty(model.SearchQuery))
            {
                model = await _searchService.PerformSearch(model);
            }

            //Store additional search parameters in TempData
            TempData["SearchQuery"] = model.SearchQuery;
            TempData["FilterExpression"] = model.FilterExpression;
            TempData["OrderByExpression"] = model.OrderByExpression;
            TempData["CountSearchResult"] = model.CountSearchResult;
            TempData["CountHighlightResult"] = model.CountHighlightResult;
            TempData["CountVectorResult"] = model.CountVectorResult;
            TempData["CountPrefixSuffix"] = model.CountPrefixSuffix;
            TempData["SearchMode"] = model.SearchMode;
            TempData["SelectedIndexes"] = model.SelectedIndexes;

            return View("Index", model); // Redirect to avoid issues with form resubmission 
        }
    }
}
