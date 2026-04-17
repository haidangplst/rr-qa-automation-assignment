using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
[TestFixture]
public class CategoryPagesTests : PlaywrightPageTest
{
    [Test]
    [Category("CategoryPagesTests")]
    
    public async Task TC_001_FilterByPopularCategory()
    {
        var testName = "TC_001_FilterByPopularCategory";
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
            var items = await popularPage.GetPopularItemsAsync();
            TestExecutionLogger.LogStepInfo($"Found {items.Count} popular items");
            TestExecutionLogger.LogStepInfo($"Item names: {string.Join(", ", items.Take(3))}...");
            if (items.Count == 0)
            {
                TestExecutionLogger.FailStep("No popular items found");
                throw new Exception("No items found on page");
            }
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-001", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-001", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }

    [Test]
    [Category("CategoryPages")]
    public async Task TC_009_FilterByTVShowsType()
    {
        Logger.TestStart("TC-009: Filter by TV Shows Type");

        try
        {
            var popularPage = new PopularPage(Page);

            Logger.Step("1", "Navigate to Popular page");
            await popularPage.NavigateToPopularAsync();

            Logger.Step("2", "Click on Movies type filter");
            await popularPage.FilterToMoviesAsync();



            Logger.Step("2", "Click on TV Shows type filter");
            await popularPage.FilterToTVShowsAsync();

            Logger.Step("3", "Verify results displayed");
            var resultsCount = await popularPage.GetResultsCountAsync();
            Logger.Assert(resultsCount > 0, "TV Show results displayed", $"Found {resultsCount} results");

            Logger.TestEnd("TC-009", resultsCount > 0);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-009", false, ex.Message);
            throw;
        }
    }

    [Test]
    [Category("CategoryPages")]
    public async Task TC_010_ToggleBetweenMoviesAndTVShows()
    {
        Logger.TestStart("TC-010: Toggle Between Movies and TV Shows");

        try
        {
            var popularPage = new PopularPage(Page);

            Logger.Step("1", "Navigate to Popular page");
            await popularPage.NavigateToPopularAsync();

            Logger.Step("2", "Filter by Movies");
            await popularPage.FilterToMoviesAsync();
            var moviesCount = await popularPage.GetResultsCountAsync();
            Logger.Info($"Movies results: {moviesCount}");

            Logger.Step("3", "Filter by TV Shows");
            await popularPage.FilterToTVShowsAsync();
            var tvShowsCount = await popularPage.GetResultsCountAsync();
            Logger.Info($"TV Shows results: {tvShowsCount}");

            Logger.Step("4", "Toggle back to Movies");
            await popularPage.FilterToMoviesAsync();
            var moviesCountAgain = await popularPage.GetResultsCountAsync();

            Logger.Assert(moviesCount > 0 && tvShowsCount > 0, "Both filters return results");
            Logger.TestEnd("TC-010", moviesCount > 0 && tvShowsCount > 0);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-010", false, ex.Message);
            throw;
        }
    }
}
