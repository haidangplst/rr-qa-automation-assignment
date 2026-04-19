using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class TopRatedPagesTests : PlaywrightPageTest
{
    [Test]
    [Category("TopRatedPagesTests")]
    [Category("Smoke")]
    public async Task TC_010_FilterByTopRatedCategory()
    {
        var testName = "TC_010_FilterByTopRatedCategory";
        TestExecutionLogger.StartTest(testName, "Smoke");
        Logger.TestStart("TC-010: Filter by Top Rated Category");

        try
        {
            var topRatedPage = new TopRatedPage(Page);

            TestExecutionLogger.RecordStep(1, "Navigate to Top Rated page");
            Logger.Step("1", "Navigate to Top Rated page");
            await topRatedPage.NavigateToTopRatedAsync();
            TestExecutionLogger.LogStepInfo("Navigated to Top Rated page");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Verify Top Rated page loaded");
            Logger.Step("2", "Verify Top Rated page loaded");
            var isTopRatedPage = await topRatedPage.IsTopRatedPageAsync();
            Logger.Assert(isTopRatedPage, "Top Rated page loaded successfully");
            TestExecutionLogger.LogStepInfo($"Top Rated page loaded: {isTopRatedPage}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Get top rated items");
            Logger.Step("3", "Get top rated items");
            var items = await topRatedPage.GetTopRatedItemsAsync();
            Logger.Info($"Found {items} top rated items");
            Logger.Assert(items > 0, "Top Rated items found");
            TestExecutionLogger.LogStepInfo($"Found {items} top rated items");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-010", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-010", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }
}
