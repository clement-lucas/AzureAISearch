using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Options;
using DocumentSearchPortal.Services.Search;
using Azure.Search.Documents.Models;
using DocumentSearchPortal.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Search.Documents;

[TestClass]
public class SearchServiceTests
{
    [TestMethod]
    public async Task PerformKeywordSearch_CallsSearchAsyncWithExpectedParameters()
    {
        // Arrange  
        var mockSearchClientWrapper = new Mock<ISearchClientWrapper>();

        // Setup the mock to acknowledge the call to SearchAsync.  
        // Note: Since we can't easily mock the return of SearchAsync with a complex Response,  
        // we'll focus on verifying the call itself and not the processing of the results.  
        // If your method processes the results in a significant way, consider abstracting that processing  
        // into a component that can be mocked or tested separately.  
        mockSearchClientWrapper.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<SearchOptions>(), It.IsAny<string>()))
                               .ReturnsAsync((string query, SearchOptions options, string index) => null); // Return null or an appropriate default  

        var searchServiceConfig = new SearchServiceConfig
        {
            IndexNameKeyword = "index-normal-fju-nonprod-jpeast-01",
            // Populate other configuration properties as needed  
        };
        var options = Options.Create(searchServiceConfig);
        var service = new SearchService(options, mockSearchClientWrapper.Object);

        var searchModel = new SearchResultViewModel
        {
            SelectedIndexes = new List<string> { "Normal" },
            SearchQuery = "clinical trials"
            // Populate other properties as necessary for the test  
        };

        // Act  
        await service.PerformSearch(searchModel);

        // Assert  
        // Verify that SearchAsync was called exactly once with the expected parameters  
        mockSearchClientWrapper.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<SearchOptions>(), searchServiceConfig.IndexNameKeyword), Times.Once);

        // Add further assertions here based on expected side effects or state changes  
    }
}
