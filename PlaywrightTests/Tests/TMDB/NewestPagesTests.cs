using PlaywrightTests.BaseTests;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class NewestPagesTests : PlaywrightPageTest
{
    [Test]
    [Category("NewestPagesTests")]
    [Category("Smoke")]
    public async Task TC_009_FilterByNewestCategory()
    {
        var testName = "TC_009_FilterByNewestCategory";
        TestExecutionLogger.StartTest(testName, "Smoke");
        Logger.TestStart("TC-009: Filter by Newest Category");

        try
        {
            var newestPage = new NewestPage(Page);

            TestExecutionLogger.RecordStep(1, "Navigate to Newest page");
            Logger.Step("1", "Navigate to Newest page");
            await newestPage.NavigateToNewestAsync();
            TestExecutionLogger.LogStepInfo("Navigated to Newest page");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(2, "Verify Newest page loaded");
            Logger.Step("2", "Verify Newest page loaded");
            var isNewestPage = await newestPage.IsNewestPageAsync();
            Logger.Assert(isNewestPage, "Newest page loaded successfully");
            TestExecutionLogger.LogStepInfo($"Newest page loaded: {isNewestPage}");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(3, "Get newest items");
            Logger.Step("3", "Get newest items");
            var items = await newestPage.GetNewestItemsAsync();
            Logger.Info($"Found {items} newest items");
            Logger.Assert(items > 0, "Newest items found");
            TestExecutionLogger.LogStepInfo($"Found {items} newest items");
            TestExecutionLogger.CompleteStep();

            TestExecutionLogger.RecordStep(4, "Get items with release dates");
            Logger.Step("4", "Get items with release dates");
            var itemsWithDates = await newestPage.GetNewestItemsWithDatesAsync();
            Logger.Info($"Retrieved {itemsWithDates.Count} items with dates");
            TestExecutionLogger.LogStepInfo($"Retrieved {itemsWithDates.Count} items with dates");
            TestExecutionLogger.CompleteStep();

            Logger.TestEnd("TC-009", true);
            TestExecutionLogger.CompleteTest(true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-009", false, ex.Message);
            TestExecutionLogger.CompleteTest(false, ex.Message, ex.StackTrace);
            throw;
        }
    }
}
