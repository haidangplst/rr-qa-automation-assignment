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
    public async Task TC_002_FilterByTrendingCategory()
    {
        Logger.TestStart("TC-002: Filter by Trending Category");

        try
        {
            var trendingPage = new TrendingPage(Page);

            Logger.Step("1", "Navigate to Trending page");
            await trendingPage.NavigateToTrendingAsync();

            Logger.Step("2", "Verify Trending page loaded");
            var isTrendingPage = await trendingPage.IsTrendingPageAsync();
            Logger.Assert(isTrendingPage, "Trending page loaded successfully");

            Logger.Step("3", "Get trending items");
            var items = await trendingPage.GetTrendingItemsAsync();
            Logger.Info($"Found {items.Count} trending items");
            Logger.Assert(items.Count > 0, "Trending items found");

            Logger.Step("4", "Get items with ratings");
            var itemsWithRatings = await trendingPage.GetTrendingItemsWithRatingsAsync();
            Logger.Info($"Retrieved {itemsWithRatings.Count} items with ratings");

            Logger.TestEnd("TC-002", true);
        }
        catch (Exception ex)
        {
            Logger.Error("Test failed", ex);
            Logger.TestEnd("TC-002", false, ex.Message);
            throw;
        }
    }
}
