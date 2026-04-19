using Microsoft.Playwright;
using PlaywrightTests.BaseTests;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.All)]
[TestFixture]
public class PopularPagesTests : PlaywrightPageTest
{

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_001_Validate_TheUIDisplayedInDefaulePage()
    {
        Logger.TestStart("TC-001: Verify The UI Displayed In Defaule Page");

        try
        {
            var homePage = new TMDBHomePage(Page);

            await homePage.WaitForDiscoveryOptionFieldsDisplayedAsync();
            await homePage.VerifyTheNavigationBarMenu();
            await homePage.VerifyTheSearchBoxDisplayed();
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-001", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_001_1_Validate_ThePagination()
    {
        Logger.TestStart("TC-001.1: Verify The Pagination");

        try
        {
            var popularPage = new PopularPage(Page);
            var homePage = new TMDBHomePage(Page);

            await homePage.WaitForDiscoveryOptionFieldsDisplayedAsync();
            await homePage.VerifyTheNavigationBarMenu();
            await homePage.VerifyTheSearchBoxDisplayed();
            await homePage.VerifyTheDefaultDisplayedPage();
            await homePage.ValidateThePageSelectorNumber();
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-001", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_001_2_Validate_ClickOnThePagination()
    {
        Logger.TestStart("TC-001.2: Verify Click On The Pagination");

        try
        {
            var popularPage = new PopularPage(Page);
            var homePage = new TMDBHomePage(Page);

            await homePage.VerifyTheDefaultDisplayedPage();
            var pageNumber = await homePage.ValidateThePageSelectorNumber();

            // Select a random page (pass 0 or negative number for random selection)
            // Or pass a specific page number like 3 for fixed page selection
            var selectedPage = await homePage.SelectPaginationPage(0); // 0 = random page

            var items = await popularPage.GetResultsCountAsync();
            TestExecutionLogger.LogStepInfo($"Found {items} popular items on page {selectedPage}");
            if (items == 0)
            {
                TestExecutionLogger.FailStep("No popular items found, please check again!");
                throw new Exception("No items found on page");
            }
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-001.2", true);
            TestExecutionLogger.CompleteTest(true);

        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-001.2", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_002_Validate_FilterByPopularCategory()
    {
        var testName = "TC_002_FilterByPopularCategory";
        TestExecutionLogger.StartTest(testName, "CategoryPages");

        try
        {
            var popularPage = new PopularPage(Page);

            // Step 1
            TestExecutionLogger.RecordStep(1, "Navigate to Popular page");
            await popularPage.NavigateToPopularAsync();
            TestExecutionLogger.LogStepInfo("Successfully navigated to Popular page");
            TestExecutionLogger.CompleteStep();

            // Step 2
            TestExecutionLogger.RecordStep(2, "Verify Popular page loaded");
            var isPopularPage = await popularPage.IsPopularPageAsync();
            TestExecutionLogger.LogStepInfo($"Page loaded: {isPopularPage}");
            if (!isPopularPage)
            {
                TestExecutionLogger.FailStep("Popular page did not load successfully");
                throw new Exception("Popular page verification failed");
            }
            TestExecutionLogger.CompleteStep();

            // Step 3
            TestExecutionLogger.RecordStep(3, "Get popular items");
            var items = await popularPage.GetResultsCountAsync();
            TestExecutionLogger.LogStepInfo($"Found {items} popular items");
            if (items == 0)
            {
                TestExecutionLogger.FailStep("No popular items found");
                throw new Exception("No items found on page");
            }
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-002", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-002", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_003_Validate_FilterByTVShowsType()
    {
        Logger.TestStart("TC-003: Filter by TV Shows Type");

        try
        {
            var popularPage = new PopularPage(Page);
            var homePage = new TMDBHomePage(Page);

            Logger.Step("1", "Navigate to Popular page");
            await popularPage.NavigateToPopularAsync();

            Logger.Step("2", "Click on Movies type filter");
            await homePage.FilterToMoviesAsync();

            Logger.Step("2", "Click on TV Shows type filter");
            await homePage.FilterToTVShowsAsync();

            Logger.Step("3", "Verify results displayed");
            var resultsCount = await popularPage.GetResultsCountAsync();
            Logger.Assert(resultsCount > 0, "TV Show results displayed", $"Found {resultsCount} results");

            Logger.TestEnd("TC-003", resultsCount > 0);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-003", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("CategoryPages")]
    public async Task TC_004_Validate_ToggleBetweenMoviesAndTVShows()
    {
        Logger.TestStart("TC-004: Toggle Between Movies and TV Shows");

        try
        {
            var popularPage = new PopularPage(Page);
            var homePage = new TMDBHomePage(Page);

            Logger.Step("1", "Navigate to Popular page");
            await popularPage.NavigateToPopularAsync();

            Logger.Step("2", "Filter by Movies");
            await homePage.FilterToMoviesAsync();
            var moviesCount = await popularPage.GetResultsCountAsync();
            Logger.Info($"Movies results: {moviesCount}");

            Logger.Step("3", "Filter by TV Shows");
            await homePage.FilterToTVShowsAsync();
            var tvShowsCount = await popularPage.GetResultsCountAsync();
            Logger.Info($"TV Shows results: {tvShowsCount}");

            Logger.Step("4", "Toggle back to Movies");
            await homePage.FilterToMoviesAsync();
            var moviesCountAgain = await popularPage.GetResultsCountAsync();

            Logger.Assert(moviesCount > 0 && tvShowsCount > 0, "Both filters return results");
            Logger.TestEnd("TC-004", moviesCount > 0 && tvShowsCount > 0);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-004", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_005_Validate_CannotDirectAccessUrlToPopularPage()
    {
        Logger.TestStart("TC-005: VerifyCannotDirectAccessUrlToPopularPage");

        try
        {
            var popularPage = new PopularPage(Page);
            Logger.Step("1", "Navigate to Example domain");
            await Page.GotoAsync("https://tmdb-discover.surge.sh/popular");
            Logger.Step("2", "Verify URL");
            var url = Page.Url;
            Logger.Info($"Current URL: {url}");
            await popularPage.VerifyThePageIsNotFoundAsync();
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-005", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_006_SearchByTitle_ExactMatch()
    {
        var searchTerm = "inception";
        var _homePage = new TMDBHomePage(Page);

        Logger.TestStart("TC-006: Search by Title - Exact Match");
        await _homePage!.NavigateToHomeAsync();

        var searchTitle = "Inception";
        Logger.Step("1", $"Search for '{searchTitle}'");
        await _homePage.SearchByTitleAsync(searchTitle);

        Logger.Step("2", "Verify search was performed");
        var searchValue = await _homePage.GetSearchValueAsync();
        Logger.Assert(searchValue?.Contains(searchTitle) ?? false, "Search input contains title");

        var resultsCount = await _homePage.GetResultsCountAsync();
        Logger.Info($"Search returned {resultsCount} results");
        var items = await _homePage.VerifyResultsItemsDescriptionAsync(searchTerm);
        Logger.Step("4", "Verify results displayed");
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_007_Validate_TheItemsHasImageErrorDisplayed()
    {
        var searchTitle = "inception";
        var _homePage = new TMDBHomePage(Page);

        Logger.TestStart("TC-007: Verify Items Have No Image Errors");

        try
        {
            await _homePage!.NavigateToHomeAsync();

            Logger.Step("1", $"Search for '{searchTitle}'");
            await _homePage.SearchByTitleAsync(searchTitle);

            Logger.Step("2", "Verify search was performed");
            var searchValue = await _homePage.GetSearchValueAsync();
            Logger.Assert(searchValue?.Contains(searchTitle) ?? false, "Search input contains title");

            var resultsCount = await _homePage.GetResultsCountAsync();
            Logger.Info($"Search returned {resultsCount} results");

            Logger.Step("3", "Get all search result items");
            var items = await _homePage.VerifyResultsItemsDescriptionAsync(searchTitle);
            Logger.Info($"Total items retrieved: {items.Count}");

            Logger.Step("4", "Verify items have no image errors");
            var itemsError = await _homePage.VerifyResultsItemsErrorimageAsync(items);

            // Check if there are any items with error images
            if (itemsError.Count > 0)
            {
                var errorMessage = $"Found {itemsError.Count} item(s) with image errors: {string.Join(", ", itemsError.Take(5))}";
                Logger.Error(errorMessage);
                Logger.TestEnd("TC-007", false, errorMessage);
                Assert.Fail(errorMessage);
            }
            else
            {
                Logger.Info($"✓ All {items.Count} items have valid images - no errors found!");
                Logger.TestEnd("TC-007", true);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-007", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("Smoke")]
    [Category("PopularPageTests")]
    public async Task TC_008_Validate_NoItemsDisplayed()
    {
        var searchTerm = "prty";
        var _homePage = new TMDBHomePage(Page);

        Logger.TestStart("TC-008: Verify No Items Displayed for Invalid Search");

        try
        {
            await _homePage!.NavigateToHomeAsync();

            Logger.Step("1", $"Search for invalid term '{searchTerm}'");
            await _homePage.SearchByTitleAsync(searchTerm);

            Logger.Step("2", "Verify search was performed");
            var searchValue = await _homePage.GetSearchValueAsync();
            Logger.Assert(searchValue?.Contains(searchTerm) ?? false, "Search input contains search term");

            Logger.Step("3", "Verify no results are displayed");
            await _homePage.VerifyNoResultDisplayed();
            var resultsCount = await _homePage.GetResultsCountAsync();
            Logger.Info($"Search returned {resultsCount} results");

            // Verify resultsCount should be 0
            if (resultsCount == 0)
            {
                Logger.Info($"✓ No results displayed as expected for invalid search term '{searchTerm}'");
                Logger.TestEnd("TC-008", true);
            }
            else
            {
                var errorMessage = $"Expected 0 results but found {resultsCount} results for search term '{searchTerm}'";
                Logger.Error(errorMessage);
                Logger.TestEnd("TC-008", false, errorMessage);
                Assert.Fail(errorMessage);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-008", false, ex.Message);
            throw;
        }
    }
}
