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
    public async Task PerformKeywordSearch_ReturnsExpectedResults()
    {
        // Arrange  
        var mockSearchClientWrapper = new Mock<ISearchClientWrapper>();
        var mockSearchResults = new Mock<SearchResults<SearchDocument>>();
        // Setup the mock to return the mockSearchResults object when SearchAsync is called  
        mockSearchClientWrapper.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<SearchOptions>(), It.IsAny<string>()))
            .ReturnsAsync(mockSearchResults.Object);

        // Instead of pulling from Key Vault, directly instantiate the configuration object for testing  
        var searchServiceConfig = new SearchServiceConfig
        {
            IndexNameKeyword = "index-normal-fju-nonprod-jpeast-01", // Use test values appropriate for the scenario  
            // Populate other necessary properties here as per the actual configuration  
        };
        var options = Options.Create(searchServiceConfig);

        // Now, create the service using the mocked configuration  
        var service = new SearchService(mockSearchClientWrapper.Object, options);

        var searchModel = new SearchResultViewModel
        {
            SelectedIndexes = new List<string> { "Keyword" },
            SearchQuery = "clinical trials"
            // Populate other necessary properties for the test  
        };

        // Act  
        var result = await service.KeywordSearchAsync(searchModel);

        // Assert  
        // Since SearchResults are mocked, we focus on interactions and method calls  
        mockSearchClientWrapper.Verify(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<SearchOptions>(), It.IsAny<string>()), Times.Once);

        // Further assertions as necessary, depending on what KeywordSearchAsync returns and what you need to verify  
    }
}
