using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.PageObjects.TMDB;
using PlaywrightTests.BaseTests;
using PlaywrightTests.Utilities;

namespace PlaywrightTests.Tests.TMDB;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class NewestPagesTests : PlaywrightPageTest
{
    [Test]
    [Category("CategoryPages")]
    [Category("Smoke")]
    public async Task TC_003_FilterByNewestCategory()
    {
        Logger.TestStart("TC-003: Filter by Newest Category");

        try
        {
            var newestPage = new NewestPage(Page);

            Logger.Step("1", "Navigate to Newest page");
            await newestPage.NavigateToNewestAsync();

            Logger.Step("2", "Verify Newest page loaded");
            var isNewestPage = await newestPage.IsNewestPageAsync();
            Logger.Assert(isNewestPage, "Newest page loaded successfully");

            Logger.Step("3", "Get newest items");
            var items = await newestPage.GetNewestItemsAsync();
            Logger.Info($"Found {items.Count} newest items");
            Logger.Assert(items.Count > 0, "Newest items found");

            Logger.Step("4", "Get items with release dates");
            var itemsWithDates = await newestPage.GetNewestItemsWithDatesAsync();
            Logger.Info($"Retrieved {itemsWithDates.Count} items with dates");

            Logger.TestEnd("TC-003", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-003", false, ex.Message);
            throw;
        }
    }
}
