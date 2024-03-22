using DocumentSearchPortal.Models;

namespace DocumentSearchPortal.Services.Search
{
    /// <summary>
    /// ISearchService
    /// </summary>
    public interface ISearchService
    {
        Task<SearchResultViewModel> PerformSearch(SearchResultViewModel model);
    }
}
