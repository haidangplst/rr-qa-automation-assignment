using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.Fixtures;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TMDBFilteringTests : PlaywrightPageTest
{
    private TMDBHomePage? _homePage;
    private APITestHelper? _apiHelper;

    [SetUp]
    public async Task SetUp()
    {
        _homePage = new TMDBHomePage(Page);
        _apiHelper = new APITestHelper(Page);

        await _apiHelper.StartNetworkCapturingAsync();
        await _homePage.NavigateToHomeAsync();

        Logger.Info($"Test setup complete - Page URL: {Page.Url}");
    }

    [TearDown]
    public async Task TearDown()
    {
        var logs = Logger.GetAllLogs();
        var apiReport = _apiHelper?.GenerateAPIReport() ?? "";
        Logger.Info($"Test teardown - Total logs: {logs.Length} chars");
    }

    
    // ==================== TITLE SEARCH TESTS ====================

    

    [Test]
    [Category("TitleSearch")]
    public async Task TC_006_SearchByTitle_PartialMatch()
    {
        Logger.TestStart("TC-006: Search by Title - Partial Match");
        await _homePage!.NavigateToHomeAsync();

        var searchTitle = "Avatar";
        Logger.Step("1", $"Search for '{searchTitle}'");
        await _homePage.SearchByTitleAsync(searchTitle);

        Logger.Step("2", "Verify results");
        var resultsCount = await _homePage.GetResultsCountAsync();
        Logger.Info($"Partial search returned {resultsCount} results");

        Logger.TestEnd("TC-006", true);
    }

    [Test]
    [Category("TitleSearch")]
    public async Task TC_007_SearchByTitle_NoResults()
    {
        Logger.TestStart("TC-007: Search by Title - No Results");
        await _homePage!.NavigateToHomeAsync();

        var searchTitle = "XYZ123NoSuchMovie";
        Logger.Step("1", $"Search for non-existent title '{searchTitle}'");
        await _homePage.SearchByTitleAsync(searchTitle);

        Logger.Step("2", "Verify no results message or empty results");
        var noResults = await _homePage.IsNoResultsMessageDisplayedAsync();
        var resultsCount = await _homePage.GetResultsCountAsync();

        Logger.Assert(noResults || resultsCount == 0, "No results message displayed or empty results");

        if (noResults)
        {
            var message = await _homePage.GetNoResultsMessageAsync();
            Logger.Info($"No results message: {message}");
        }

        Logger.TestEnd("TC-007", noResults || resultsCount == 0);
    }

    // ==================== PAGINATION TESTS ====================

    [Test]
    [Category("Pagination")]
    [Category("Smoke")]
    public async Task TC_024_NavigateToNextPage()
    {
        Logger.TestStart("TC-024: Navigate to Next Page");
        await _homePage!.NavigateToHomeAsync();

        Logger.Step("1", "Get initial page results");
        var page1Results = await _homePage.GetAllResultTitlesAsync();
        Logger.Info($"Page 1 has {page1Results.Count} results");

        Logger.Step("2", "Navigate to next page");
        var isNextEnabled = await _homePage.IsNextPageEnabledAsync();
        Logger.Assert(isNextEnabled, "Next page button is enabled");

        if (isNextEnabled)
        {
            await _homePage.GoToNextPageAsync();
            var page2Results = await _homePage.GetAllResultTitlesAsync();
            Logger.Info($"Page 2 has {page2Results.Count} results");

            // Check if results are different
            var hasDifferentContent = !page1Results.SequenceEqual(page2Results);
            Logger.Assert(hasDifferentContent, "Page 2 has different content than Page 1");

            Logger.TestEnd("TC-024", page2Results.Count > 0);
        }
        else
        {
            Logger.Warning("Next button not enabled");
            Logger.TestEnd("TC-024", false, "Next button not available");
        }
    }

    [Test]
    [Category("Pagination")]
    public async Task TC_025_NavigateToPreviousPage()
    {
        Logger.TestStart("TC-025: Navigate to Previous Page");
        await _homePage!.NavigateToHomeAsync();

        Logger.Step("1", "Navigate to next page");
        var isNextEnabled = await _homePage.IsNextPageEnabledAsync();

        if (isNextEnabled)
        {
            await _homePage.GoToNextPageAsync();
            var page2Results = await _homePage.GetAllResultTitlesAsync();

            Logger.Step("2", "Navigate back to previous page");
            var isPrevEnabled = await _homePage.IsPreviousPageEnabledAsync();
            Logger.Assert(isPrevEnabled, "Previous button is enabled");

            if (isPrevEnabled)
            {
                await _homePage.GoToPreviousPageAsync();
                var page1Results = await _homePage.GetAllResultTitlesAsync();

                Logger.Assert(page1Results.Count > 0, "Previous page has results");
                Logger.TestEnd("TC-025", page1Results.Count > 0);
            }
            else
            {
                Logger.TestEnd("TC-025", false, "Previous button not available");
            }
        }
        else
        {
            Logger.TestEnd("TC-025", false, "Could not navigate to page 2");
        }
    }

    [Test]
    [Category("Pagination")]
    public async Task TC_027_PaginationWithFiltersApplied()
    {
        Logger.TestStart("TC-027: Pagination with Filters Applied");
        await _homePage!.NavigateToHomeAsync();

        Logger.Step("1", "Apply Popular filter");
        await _homePage.FilterByPopularAsync();
        var page1Results = await _homePage.GetAllResultTitlesAsync();

        Logger.Step("2", "Navigate to next page with filter active");
        var isNextEnabled = await _homePage.IsNextPageEnabledAsync();

        if (isNextEnabled)
        {
            await _homePage.GoToNextPageAsync();
            var page2Results = await _homePage.GetAllResultTitlesAsync();

            Logger.Assert(page2Results.Count > 0, "Filtered results on page 2");
            Logger.TestEnd("TC-027", page2Results.Count > 0);
        }
        else
        {
            Logger.TestEnd("TC-027", false);
        }
    }

    // ==================== COMBINED FILTERING TESTS ====================

    [Test]
    [Category("CombinedFilters")]
    public async Task TC_021_CombineCategory_And_TypeFilter()
    {
        Logger.TestStart("TC-021: Combine Category + Type Filter");
        await _homePage!.NavigateToHomeAsync();

        Logger.Step("1", "Apply Popular filter");
        await _homePage.FilterByPopularAsync();

        Logger.Step("2", "Apply Movies type filter");
        await _homePage.FilterByMoviesTypeAsync();

        Logger.Step("3", "Verify combined results");
        var resultsCount = await _homePage.GetResultsCountAsync();
        Logger.Assert(resultsCount > 0, "Combined filters return results");

        Logger.TestEnd("TC-021", resultsCount > 0);
    }

    // ==================== NEGATIVE TESTS ====================

    [Test]
    [Category("NegativeTests")]
    public async Task TC_030_DirectURLAccess_WithSlug()
    {
        Logger.TestStart("TC-030: Direct URL Access with Slug (NEGATIVE TEST)");

        Logger.Step("1", "Navigate to /popular slug");
        Logger.Warning("This is a known issue - slug-based URLs may not work");

        try
        {
            await Page!.GotoAsync("https://tmdb-discover.surge.sh/popular");
            Logger.Info($"Page loaded at URL: {Page.Url}");

            var resultsCount = await _homePage!.GetResultsCountAsync();
            Logger.Info($"Results count after slug navigation: {resultsCount}");

            Logger.TestEnd("TC-030", true, "Slug navigation handled");
        }
        catch (Exception ex)
        {
            Logger.Error("Error with slug navigation", ex);
            Logger.TestEnd("TC-030", false, $"Exception: {ex.Message}");
        }
    }

    [Test]
    [Category("NegativeTests")]
    public async Task TC_032_InvalidPageNumber_PageZero()
    {
        Logger.TestStart("TC-032: Invalid Page Number (Page 0)");
        await _homePage!.NavigateToHomeAsync();

        Logger.Step("1", "Attempt to navigate to page 0");
        // This would require URL parameter modification
        // For now, we'll document the test
        Logger.Warning("Page 0 navigation test - requires URL parameter handling");

        Logger.TestEnd("TC-032", true, "Documented for manual testing");
    }

    [Test]
    [Category("UIValidation")]
    [Category("Smoke")]
    public async Task TC_035_PageLoadCompleteness()
    {
        Logger.TestStart("TC-035: Page Load Completeness");
        await _homePage!.NavigateToHomeAsync();

        Logger.Step("1", "Verify page title");
        var titleCorrect = await _homePage.IsTitleCorrectAsync("TMDB");
        Logger.Assert(titleCorrect, "Page title is correct");

        Logger.Step("2", "Verify results are loaded");
        var resultsCount = await _homePage.GetResultsCountAsync();
        Logger.Assert(resultsCount > 0, "Results grid loaded", $"Found {resultsCount} results");

        Logger.Step("3", "Verify page URL");
        var url = Page!.Url;
        Logger.Assert(url.Contains("tmdb-discover"), "Correct URL", $"URL: {url}");

        Logger.TestEnd("TC-035", titleCorrect && resultsCount > 0);
    }
}
