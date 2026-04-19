using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TrendPagesTests : PlaywrightPageTest
{

    [Test]
    [Category("CategoryPages")]
    [Category("Smoke")]
    public async Task TC_011_FilterByTrendingCategory()
    {
        var testName = "TC_011_FilterByTrendingCategory";
        TestExecutionLogger.StartTest(testName, "Smoke");
        Logger.TestStart("TC-011: Filter by Trending Category");

        try
        {
            var trendingPage = new TrendingPage(Page);

            TestExecutionLogger.RecordStep(1, "Navigate to Trending page");
            Logger.Step("1", "Navigate to Trending page");
            await trendingPage.NavigateToTrendingAsync();
            TestExecutionLogger.LogStepInfo("Navigated to Trending page");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Verify Trending page loaded");
            Logger.Step("2", "Verify Trending page loaded");
            var isTrendingPage = await trendingPage.IsTrendingPageAsync();
            Logger.Assert(isTrendingPage, "Trending page loaded successfully");
            TestExecutionLogger.LogStepInfo($"Trending page loaded: {isTrendingPage}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Get trending items");
            Logger.Step("3", "Get trending items");
            var items = await trendingPage.GetTrendingItemsAsync();
            Logger.Info($"Found {items} trending items");
            Logger.Assert(items > 0, "Trending items found");
            TestExecutionLogger.LogStepInfo($"Found {items} trending items");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Get items with ratings");
            Logger.Step("4", "Get items with ratings");
            var itemsWithRatings = await trendingPage.GetTrendingItemsWithRatingsAsync();
            Logger.Info($"Retrieved {itemsWithRatings.Count} items with ratings");
            TestExecutionLogger.LogStepInfo($"Retrieved {itemsWithRatings.Count} items with ratings");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-011", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-011", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }
}
